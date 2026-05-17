"use strict";

// ---------------------------------------------------------------------------
// State
// ---------------------------------------------------------------------------
let indexData = null;     // data/index.json
let currentDoc = null;    // data/<testCases>.json
let currentNetwork = null;
let currentResult = null; // selected testCaseResults entry
let currentStep = 0;
let playTimer = null;
let eqLayer = null;       // HTML overlay holding the per-synapse equations
let eqDivs = {};          // synapse index -> equation <div>

const el = (id) => document.getElementById(id);

// ---------------------------------------------------------------------------
// Number formatting
// ---------------------------------------------------------------------------
function fmt(value) {
  if (value === null || value === undefined) return "-";
  if (Number.isInteger(value)) return String(value);
  const rounded = Math.round(value * 1000) / 1000;
  if (Number.isInteger(rounded)) return String(rounded);
  return rounded.toFixed(3).replace(/0+$/, "").replace(/\.$/, "");
}

// ---------------------------------------------------------------------------
// Activation functions (input domain -5..+5) -> per-neuron SVG mini-plot
// ---------------------------------------------------------------------------
function activationName(code) {
  switch (code) {
    case "L": return "Linear";
    case "T": return "Threshold";
    case "S": return "Squashing";
    case "Σ": return "Sigmoid";
    case "H": return "Tanh";
    default: return code || "?";
  }
}

function activationFn(code) {
  switch (code) {
    case "T": return (x) => Math.min(Math.max(-1, x), 1);
    case "S": return (x) => Math.min(Math.max(0, x), 1);
    case "Σ": return (x) => 1 / (1 + Math.exp(-x));
    case "H": return (x) => Math.tanh(x);
    default: return (x) => x;
  }
}

// Same fixed scale for every activation function so they are directly
// comparable: input -5..+5 on x, output -5..+5 on y.
function activationRange() {
  return [-5.2, 5.2];
}

function activationSvg(index, code) {
  const W = 150, H = 116;
  const padX = 12, padTop = 32, padBottom = 16;
  const px = padX, pw = W - 2 * padX;
  const py = padTop, ph = H - padTop - padBottom;
  const xMin = -5, xMax = 5;
  const range = activationRange();
  const yMin = range[0], yMax = range[1];
  const fn = activationFn(code);

  const sx = (x) => px + (x - xMin) / (xMax - xMin) * pw;
  const sy = (y) => py + (yMax - y) / (yMax - yMin) * ph;

  const samples = 64;
  let d = "";
  for (let i = 0; i <= samples; i++) {
    const x = xMin + (xMax - xMin) * i / samples;
    let y = fn(x);
    if (y > yMax) y = yMax;
    if (y < yMin) y = yMin;
    d += (i === 0 ? "M" : "L") + sx(x).toFixed(1) + " " + sy(y).toFixed(1) + " ";
  }

  const zeroX = sx(0).toFixed(1);
  const zeroY = sy(0).toFixed(1);

  // Guidelines every 1 unit on both axes; the zero axes are drawn brighter.
  let grid = "";
  for (let gx = -5; gx <= 5; gx++) {
    const gX = sx(gx).toFixed(1);
    grid += '<line x1="' + gX + '" y1="' + py + '" x2="' + gX + '" y2="' + (py + ph) + '" stroke="#26344f" stroke-width="0.5"/>';
  }
  for (let gy = -5; gy <= 5; gy++) {
    const gY = sy(gy).toFixed(1);
    grid += '<line x1="' + px + '" y1="' + gY + '" x2="' + (px + pw) + '" y2="' + gY + '" stroke="#26344f" stroke-width="0.5"/>';
  }
  const axes =
    '<line x1="' + zeroX + '" y1="' + py + '" x2="' + zeroX + '" y2="' + (py + ph) + '" stroke="#4a5d8a" stroke-width="1"/>'
    + '<line x1="' + px + '" y1="' + zeroY + '" x2="' + (px + pw) + '" y2="' + zeroY + '" stroke="#4a5d8a" stroke-width="1"/>';

  const svg =
    '<svg xmlns="http://www.w3.org/2000/svg" width="' + W + '" height="' + H + '" viewBox="0 0 ' + W + ' ' + H + '">'
    + '<rect x="1" y="1" width="' + (W - 2) + '" height="' + (H - 2) + '" rx="12" fill="#16223a" stroke="#3a4a6c" stroke-width="1"/>'
    + '<text x="' + (W / 2) + '" y="15" fill="#e8eefc" font-family="Consolas,monospace" font-size="12" font-weight="700" text-anchor="middle">Neuron ' + index + '</text>'
    + '<text x="' + (W / 2) + '" y="27" fill="#93a3c4" font-family="Consolas,monospace" font-size="10" text-anchor="middle">' + activationName(code) + '</text>'
    + '<rect x="' + px + '" y="' + py + '" width="' + pw + '" height="' + ph + '" fill="#0e1726" stroke="#2a3a5c"/>'
    + grid
    + axes
    + '<path d="' + d.trim() + '" fill="none" stroke="#5b9dff" stroke-width="2.5" stroke-linejoin="round" stroke-linecap="round"/>'
    + '<text x="' + (px + 2) + '" y="' + (py + 9) + '" fill="#6b7a9c" font-family="Consolas,monospace" font-size="8">+5</text>'
    + '<text x="' + (px + 2) + '" y="' + (py + ph - 3) + '" fill="#6b7a9c" font-family="Consolas,monospace" font-size="8">-5</text>'
    + '<text x="' + (W / 2) + '" y="' + (H - 4) + '" fill="#6b7a9c" font-family="Consolas,monospace" font-size="9" text-anchor="middle">input/output -5 .. +5</text>'
    + '</svg>';

  return "data:image/svg+xml;charset=utf-8," + encodeURIComponent(svg);
}

// ---------------------------------------------------------------------------
// Cytoscape graph
// ---------------------------------------------------------------------------
let cy = null;

function initCytoscape() {
  cy = cytoscape({
    container: el("cy"),
    minZoom: 0.2,
    maxZoom: 3,
    wheelSensitivity: 0.25,
    style: [
      {
        selector: "node",
        style: {
          "label": "data(label)",
          "text-valign": "center",
          "text-halign": "center",
          "text-wrap": "wrap",
          "color": "#e8eefc",
          "font-size": 13,
          "font-weight": 600,
          "font-family": "Consolas, monospace",
          "width": "label",
          "height": "label",
          "padding": "10px",
          "shape": "round-rectangle",
          "border-width": 1,
          "border-color": "rgba(255,255,255,0.18)"
        }
      },
      {
        selector: "node.neuron",
        style: {
          "background-color": "#21345a",
          "font-size": 11,
          "padding": "5px",
          "border-width": 1,
          "border-color": "#3a4a6c"
        }
      },
      {
        selector: "node.neuron.expanded",
        style: {
          "label": "",
          "width": 150,
          "height": 116,
          "padding": "0px",
          "shape": "rectangle",
          "background-opacity": 0,
          "background-image": "data(bg)",
          "background-fit": "cover",
          "border-width": 0
        }
      },
      {
        selector: "node.bias",
        style: { "background-color": "#f5b342", "color": "#0e1726" }
      },
      {
        selector: "node.input",
        style: { "background-color": "#8a93a8", "color": "#0e1726" }
      },
      {
        selector: "node.output",
        style: { "background-color": "#8a93a8", "color": "#0e1726" }
      },
      { selector: "node.output.mismatch", style: { "background-color": "#ff6b81", "color": "#fff" } },
      { selector: "node.output.match", style: { "background-color": "#36d399", "color": "#0e1726" } },
      {
        selector: "edge",
        style: {
          // Synapse text is rendered by the HTML equation overlay, not here.
          "width": 2,
          "line-color": "#3a4a6c",
          "target-arrow-color": "#3a4a6c",
          "target-arrow-shape": "triangle",
          "curve-style": "bezier"
        }
      },
      {
        selector: "edge.active",
        style: { "line-color": "#5b9dff", "target-arrow-color": "#5b9dff", "width": 3, "color": "#e8eefc" }
      },
      {
        selector: "edge.loop",
        style: {
          "curve-style": "bezier",
          "loop-direction": "0deg",
          "loop-sweep": "-55deg",
          "control-point-step-size": 80,
          "z-compound-depth": "top"
        }
      }
    ]
  });

  cy.on("ready", () => cy.fit(undefined, 40));

  // The activation-function graph is shown only on demand: click a neuron to
  // expand it into its curve, click again to collapse back to a compact node.
  cy.on("tap", "node.neuron", (event) => event.target.toggleClass("expanded"));

  // HTML overlay for the per-synapse equations (Cytoscape labels are single
  // colour; this lets each part of the equation have its own colour).
  eqLayer = document.createElement("div");
  eqLayer.id = "edgeOverlay";
  el("cy").appendChild(eqLayer);
  cy.on("render pan zoom resize", positionEqs);
}

// Convert each edge's model midpoint to a rendered pixel position and glue
// its equation div there. Cheap enough to run on every render for these
// small networks.
function positionEqs() {
  if (!eqLayer) return;
  const zoom = cy.zoom();
  const pan = cy.pan();
  for (const synIdx in eqDivs) {
    const edge = cy.getElementById("s" + synIdx);
    if (edge.empty()) continue;
    const mid = edge.midpoint();
    const div = eqDivs[synIdx];
    div.style.left = (mid.x * zoom + pan.x) + "px";
    div.style.top = (mid.y * zoom + pan.y) + "px";
  }
}

function buildGraph(network) {
  const elements = [];
  const portCreated = {};

  for (const node of network.nodes) {
    const isBias = node.type === "Bias";
    elements.push({
      data: {
        id: "n" + node.index,
        label: isBias
          ? "Bias " + node.index + "\n≡ 1"
          : "#" + node.index + "\n" + node.activation,
        bg: isBias ? "" : activationSvg(node.index, node.activation)
      },
      classes: isBias ? "bias" : "neuron"
    });
  }

  for (const syn of network.synapses) {
    let source;
    let target;

    if (syn.role === "input") {
      source = "in" + syn.inputIndex;
      if (!portCreated[source]) {
        elements.push({ data: { id: source, label: "in" + syn.inputIndex }, classes: "input" });
        portCreated[source] = true;
      }
      target = "n" + syn.to;
    } else if (syn.role === "output") {
      source = "n" + syn.from;
      target = "out" + syn.outputIndex;
      if (!portCreated[target]) {
        elements.push({ data: { id: target, label: "out" + syn.outputIndex }, classes: "output" });
        portCreated[target] = true;
      }
    } else {
      source = "n" + syn.from;
      target = "n" + syn.to;
    }

    elements.push({
      data: { id: "s" + syn.index, source: source, target: target },
      classes: source === target ? "loop" : ""
    });
  }

  cy.elements().remove();
  cy.add(elements);
  cy.layout({
    name: "dagre",
    rankDir: "LR",
    nodeSep: 40,
    rankSep: 140,
    edgeSep: 20,
    fit: true,
    padding: 40
  }).run();

  // Rebuild the equation overlay (one div per synapse).
  eqLayer.innerHTML = "";
  eqDivs = {};
  for (const syn of network.synapses) {
    const div = document.createElement("div");
    div.className = "eq";
    eqLayer.appendChild(div);
    eqDivs[syn.index] = div;
  }
  positionEqs();
}

// ---------------------------------------------------------------------------
// Stepping
// ---------------------------------------------------------------------------
function snapshotCount() {
  return currentResult ? currentResult.snapshots.length : 0;
}

function updateStep(step) {
  currentStep = step;
  const snapshot = currentResult.snapshots[step];
  const network = currentNetwork;

  cy.batch(() => {
    for (const syn of network.synapses) {
      const potential = snapshot[syn.index];
      const edge = cy.getElementById("s" + syn.index);
      const hasMult = syn.multiplier !== null && syn.multiplier !== undefined;
      const output = hasMult ? potential * syn.multiplier : potential;

      const div = eqDivs[syn.index];
      if (div) {
        if (hasMult) {
          div.innerHTML =
            '<span class="eq-prev">' + fmt(potential) + '</span>'
            + '<span class="eq-op">×</span>'
            + '<span class="eq-mul">' + fmt(syn.multiplier) + '</span>'
            + '<span class="eq-op">=</span>'
            + '<span class="eq-out">' + fmt(output) + '</span>';
        } else {
          div.innerHTML = '<span class="eq-out">' + fmt(output) + '</span>';
        }
        div.className = Math.abs(output) < 1e-9 ? "eq eq-dim" : "eq";
      }

      if (Math.abs(potential) > 1e-9) edge.addClass("active");
      else edge.removeClass("active");

      if (syn.role === "input") {
        cy.getElementById("in" + syn.inputIndex).data("label", "in" + syn.inputIndex + " = " + fmt(potential));
      } else if (syn.role === "output") {
        const expected = currentResult.expected[syn.outputIndex];
        const portNode = cy.getElementById("out" + syn.outputIndex);
        portNode.data("label", "out" + syn.outputIndex + " = " + fmt(potential));
        portNode.removeClass("match mismatch");
        portNode.addClass(Math.abs(potential - expected) > 0.001 ? "mismatch" : "match");
      }
    }
  });

  el("stepSlider").value = String(step);
  el("stepLabel").textContent = "step " + step + " / " + (snapshotCount() - 1);
}

function stopPlay() {
  if (playTimer !== null) {
    clearInterval(playTimer);
    playTimer = null;
    el("playButton").innerHTML = "&#9654; Play";
  }
}

function togglePlay() {
  if (playTimer !== null) {
    stopPlay();
    return;
  }
  if (currentStep >= snapshotCount() - 1) updateStep(0);
  el("playButton").innerHTML = "&#10073;&#10073; Pause";
  playTimer = setInterval(() => {
    if (currentStep >= snapshotCount() - 1) {
      stopPlay();
      return;
    }
    updateStep(currentStep + 1);
  }, 900);
}

// ---------------------------------------------------------------------------
// Rendering selections
// ---------------------------------------------------------------------------
function renderNetwork(network) {
  currentNetwork = network;
  stopPlay();

  el("networkComment").textContent = network.comment || "(no comment provided in web-export.config)";

  const synapseCount = network.synapses.length;
  el("networkMeta").innerHTML = "";
  const metas = [
    "Nodes: " + network.nodes.length,
    "Synapses: " + synapseCount,
    "Propagations: " + network.propagations,
    network.solves ? "Solves: all cases" : "Solves: partial"
  ];
  for (const text of metas) {
    const span = document.createElement("span");
    span.textContent = text;
    el("networkMeta").appendChild(span);
  }

  el("networkText").textContent = network.networkText;

  buildGraph(network);

  const tcSelect = el("testCaseSelect");
  tcSelect.innerHTML = "";
  network.testCaseResults.forEach((result, i) => {
    const option = document.createElement("option");
    option.value = String(i);
    const flag = result.pass ? "✓" : "✗";
    option.textContent = "#" + (i + 1) + "  [" + result.inputs.join(", ") + "] → ["
      + result.expected.join(", ") + "]  " + flag;
    tcSelect.appendChild(option);
  });

  if (network.testCaseResults.length > 0) {
    tcSelect.value = "0";
    renderTestCase(0);
  }
}

function renderTestCase(resultIndex) {
  currentResult = currentNetwork.testCaseResults[resultIndex];
  stopPlay();

  el("ioInputs").textContent = currentResult.inputs.join(", ");
  el("ioExpected").textContent = currentResult.expected.join(", ");

  const actualCell = el("ioActual");
  actualCell.innerHTML = "";
  currentResult.actual.forEach((value, i) => {
    if (i > 0) actualCell.appendChild(document.createTextNode(", "));
    const span = document.createElement("span");
    span.textContent = fmt(value);
    const expected = currentResult.expected[i];
    if (expected === undefined || Math.abs(value - expected) > 0.001) span.className = "mismatch";
    actualCell.appendChild(span);
  });
  if (currentResult.actual.length === 0) actualCell.textContent = "(network shape too small for this case)";

  const badge = el("passBadge");
  badge.className = "badge " + (currentResult.pass ? "pass" : "fail");
  badge.textContent = currentResult.pass ? "PASS — within 0.001" : "FAIL — output differs";

  const slider = el("stepSlider");
  slider.max = String(Math.max(0, snapshotCount() - 1));
  slider.value = "0";
  updateStep(0);
}

async function loadDocument(file) {
  const response = await fetch("data/" + file, { cache: "no-store" });
  if (!response.ok) throw new Error("HTTP " + response.status);
  currentDoc = await response.json();

  const networkSelect = el("networkSelect");
  networkSelect.innerHTML = "";
  currentDoc.networks.forEach((network, i) => {
    const option = document.createElement("option");
    option.value = String(i);
    const flag = network.solves ? "✓ solves all" : "✗ partial";
    option.textContent = network.network + "  (" + flag + ")";
    networkSelect.appendChild(option);
  });

  if (currentDoc.networks.length > 0) {
    networkSelect.value = "0";
    renderNetwork(currentDoc.networks[0]);
  }
}

// ---------------------------------------------------------------------------
// Wiring
// ---------------------------------------------------------------------------
function wireEvents() {
  el("testCasesSelect").addEventListener("change", (e) => {
    const entry = indexData.entries[Number(e.target.value)];
    loadDocument(entry.file).catch(showLoadError);
  });

  el("networkSelect").addEventListener("change", (e) => {
    renderNetwork(currentDoc.networks[Number(e.target.value)]);
  });

  el("testCaseSelect").addEventListener("change", (e) => {
    renderTestCase(Number(e.target.value));
  });

  el("stepSlider").addEventListener("input", (e) => {
    stopPlay();
    updateStep(Number(e.target.value));
  });

  el("firstButton").addEventListener("click", () => { stopPlay(); updateStep(0); });
  el("prevButton").addEventListener("click", () => { stopPlay(); if (currentStep > 0) updateStep(currentStep - 1); });
  el("nextButton").addEventListener("click", () => {
    stopPlay();
    if (currentStep < snapshotCount() - 1) updateStep(currentStep + 1);
  });
  el("lastButton").addEventListener("click", () => { stopPlay(); updateStep(snapshotCount() - 1); });
  el("playButton").addEventListener("click", togglePlay);
  el("fitButton").addEventListener("click", () => cy.fit(undefined, 40));

  el("downloadButton").addEventListener("click", () => {
    if (!currentNetwork) return;
    const blob = new Blob([currentNetwork.networkText], { type: "text/plain" });
    const url = URL.createObjectURL(blob);
    const link = document.createElement("a");
    link.href = url;
    link.download = currentNetwork.network + ".Network.txt";
    document.body.appendChild(link);
    link.click();
    link.remove();
    URL.revokeObjectURL(url);
  });

  document.addEventListener("keydown", (e) => {
    if (e.target && e.target.tagName === "SELECT") return;
    if (e.key === "ArrowLeft") { stopPlay(); if (currentStep > 0) updateStep(currentStep - 1); }
    else if (e.key === "ArrowRight") { stopPlay(); if (currentStep < snapshotCount() - 1) updateStep(currentStep + 1); }
    else if (e.key === " ") { e.preventDefault(); togglePlay(); }
  });
}

function buildLegend() {
  const items = [
    ["#21345a", "Neuron — click to expand its activation curve (−5…+5)"],
    ["#f5b342", "Bias (constant 1)"],
    ["#8a93a8", "Input / output port"],
    ["#36d399", "Output matches expected"],
    ["#ff6b81", "Output mismatch"],
    ["#5bd6c0", "Synapse equation: previous output"],
    ["#c08bff", "× multiplier"],
    ["#7aa2ff", "= value delivered"]
  ];
  const legend = el("legend");
  for (const [color, text] of items) {
    const span = document.createElement("span");
    span.innerHTML = '<span class="swatch" style="background:' + color + '"></span>' + text;
    legend.appendChild(span);
  }
}

function showLoadError(error) {
  const box = el("loadError");
  box.hidden = false;
  box.textContent = "Could not load data (" + error.message
    + "). Browsers block local file:// fetch — run web/serve.ps1 (or any static server) and open http://localhost:8080/ instead.";
}

async function main() {
  initCytoscape();
  buildLegend();
  wireEvents();

  try {
    const response = await fetch("data/index.json", { cache: "no-store" });
    if (!response.ok) throw new Error("HTTP " + response.status);
    indexData = await response.json();
  } catch (error) {
    showLoadError(error);
    return;
  }

  const tcSelect = el("testCasesSelect");
  indexData.entries.forEach((entry, i) => {
    const option = document.createElement("option");
    option.value = String(i);
    option.textContent = entry.testCases;
    tcSelect.appendChild(option);
  });

  if (indexData.entries.length > 0) {
    tcSelect.value = "0";
    await loadDocument(indexData.entries[0].file).catch(showLoadError);
  } else {
    showLoadError(new Error("index.json has no entries"));
  }
}

main();
