# MssShipping — Magna Seating Mississauga (MSS)

**DFX project.** If the DFX framework context was not auto-injected at session start (look for
an "=== DFX framework context" block in your context), read
`C:\Customers\DacQuest\FrameWorks\DFX20\Source\DFX-CONTEXT.md` before doing any work here.
(The inject hook only runs in sessions launched from this repo; sessions launched from
C:\Customers must read the file explicitly — cross-repo `@imports` do not expand.)

Shipping system for Mississauga.
- Solution `MssShipping.sln`; namespace prefix `Mss.*`; site config
  `Configuration\MssShipping.config`.
- Hosts: ShippingService, ShippingManager/ShippingAdmin, ShippingAgent; operator client
  LoadDirectorClient; Mss.OfflineSystemEventViewer.

## Team & safety rules
- This system drives live plant equipment (PLCs, cranes) and a production SQL Server
  database. Never run against, or edit deployed config in, this site's `System\` folder
  (the deployed runtime, which lives outside this repo) unless explicitly asked.
- The `C:\Customers\<Customer>\<Site>\<System>\Source` layout is load-bearing — projects
  reference the framework's built assemblies by relative HintPath — so the folder layout
  must not be rearranged.

## Project-specific notes
- (site quirks, known issues, deployment notes — add as they come up)

## Keeping this file fresh
Update this file in the same session whenever source changes alter the facts above **or** your
understanding of this system's purpose, behavior, or quirks evolves through working here — even
in sessions that change no code.