# Solutions Explorer (web)

An independent, static page that lets you explore pre-calculated neural-network
solutions for unit-test cases: pick a test-case list, pick a successful network,
pick a test case, then step through the propagations one by one and watch each
synapse's multiplier and live value on an interactive graph. You can also view
and download the network in the exact text format the program accepts.

Everything here is static — there is no server-side computation. All values are
pre-computed by the .NET model.

## Viewing

Browsers block `fetch()` of local files when you open `index.html` via
`file://`, so serve the folder over HTTP:

```powershell
pwsh web/serve.ps1        # then open http://localhost:8080/
```

Any static server works equally well (e.g. `python -m http.server 8080`
from this directory). Firefox can sometimes open `index.html` directly.

## Regenerating the data

The `data/` folder is generated; do not edit it by hand.

1. Edit [`web-export.config`](web-export.config) — one line per
   `testCases  network  propagations  # comment`. The comment is shown on the page.
2. Run the generator:

   ```powershell
   pwsh web/generate-web.ps1
   ```

   It builds the solution, drives the console app over piped stdin, and writes
   one `data/<testCases>.json` per test-cases file plus `data/index.json`.
   A PASS/FAIL summary is printed (a combination "solves" when every test case
   is within 0.001 of the expected output). Networks that only partially solve
   are still published and shown with per-case badges, so you can tune
   `propagations` and re-run until it goes green.

## Layout

```
web/
  index.html  style.css  app.js   the page
  vendor/                          cytoscape + dagre (offline, no CDN)
  data/                            generated: index.json + <testCases>.json
  serve.ps1                        minimal local static server
  generate-web.ps1                 data generator (drives the console app)
  web-export.config                combinations to export (edit this)
```
