# MssShipping — Magna Seating Mississauga (MSS)

**DFX project.** If the DFX framework context was not auto-injected at session start (look for
an "=== DFX framework context" block in your context), read
`C:\Customers\DacQuest\FrameWorks\DFX20\Source\.claude\DFX-CONTEXT.md` before doing any work here.
(The inject hook only runs in sessions launched from this repo; sessions launched from
C:\Customers must read the file explicitly — cross-repo `@imports` do not expand.)

Shipping system for Mississauga.
- Solution `MssShipping.sln`; namespace prefix `Mss.*`; site config
  `Configuration\MssShipping.config` (variant: `_LOCAL`, not touched since 4523400 and behind
  the main config).
- Hosts: ShippingService, ShippingManager/ShippingAdmin, ShippingAgent; operator client
  LoadDirectorClient (instances Upper, Lower; there is no TrailerLoadClient project);
  Mss.OfflineSystemEventViewer.

## Team & safety rules
- This system drives live plant equipment (PLCs, cranes) and a production SQL Server
  database. Never run against, or edit deployed config in, this site's `System\` folder
  (the deployed runtime, which lives outside this repo) unless explicitly asked.
- The `C:\Customers\<Customer>\<Site>\<System>\Source` layout is load-bearing — projects
  reference the framework's built assemblies by relative HintPath — so the folder layout
  must not be rearranged.

## Project-specific notes
Verified against the code at aca27d5.
- **Committed config is not a production config.** At HEAD the connection strings use the
  `Dev*` aliases (localhost) and `ServerIPAddress` is loopback; `Live*` aliases and
  `ProductionServerIPAddress` sit alongside, and aca27d5 switched back from them. The deployed
  config was not compared.
- **Builds write to `System\`:** every project has OutputPath `..\..\System\DebugBuild\` (or
  `ReleaseBuild\`) and a post-build copy to `..\System\$(ConfigurationName)`. Don't compile-check
  without asking. Apps load `..\Configuration\MssShipping.config` (App.config); label definitions
  come from `..\Labels`, which is not in this repo.
- Plant: 4 cranes (`Mss.Operations.Crane`), 1104 bins (2 sides x 23 x 6 per crane), 3 Assignment
  operations; Router x4, LoadDirector, RecircRouter, RecircBuffer, Purge, Transfer on each of the
  Upper and Lower levels; TrailerLoadA/B. Sizing is commented out of the config. All PLC devices
  share one `PlcIPAddress` over ABLogix.
- Slug flow: two slugs, `SlugA`/`SlugB` (`SlugManager` picks the target for broadcast release),
  `LoadSize` 54 (27 per level, 3 lanes). When a load completes, TrailerLoad needs a valid trailer
  number from its PLC tag (else AwaitingTrailerNumber), then tries `TryAutoAcceptLoad` and prints
  the load label.
- MES (`MesInterfaceService`, polled): broadcast 300 s, hold codes 3600 s, status changes 30 s,
  broadcast coverage 60 s (wrapper default). Coverage does a delete + insert on
  `SHIP_BroadcastCoverage` over `MesConnectionString`; no script in `MesInterfaceSqlScripts`
  creates that table.
- Crane pick order (alias `CranePickFunctionPriority`): PurgePick, Stack1Pick, Stack2Pick,
  LoadPick, Audit.
- Recirc buffer off: both LoadDirectors set the literal `RecircBufferEnabled:=false` (the alias of
  that name is no longer read) and `AutoReleaseNonLoadPallets:=false`.
- Label printing is manual-printer only: `Upper/LowerAutoLabelPrinterEnabled` are false and the auto
  printers are out of the LoadDirector device sets. Custom driver: `Mss.Operations.IplPrinterDriver`.
- `PlcTagService` is the one service with StartMode Manual. The `PlcTags` collection it feeds is
  empty until it is started.
- DFX: references bin\Debug assemblies by HintPath (.NET 4.7.2). Every ABLogix device sets
  `RoutePath:=@:PlcRoutePath` (0) and `UpdatePeriodMilliseconds:=500`, so DFX's changed defaults
  for those two parameters (ecd61dd and earlier) don't affect it.

## Keeping this file fresh
Update this file in the same session whenever source changes alter the facts above **or** your
understanding of this system's purpose, behavior, or quirks evolves through working here — even
in sessions that change no code.