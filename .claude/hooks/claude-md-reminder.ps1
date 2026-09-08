# PostToolUse (Edit|Write) hook - keep THIS repo's CLAUDE.md fresh.
# After the first noteworthy source change in a session, remind once to check whether the
# repo's CLAUDE.md (at the repo root) still matches reality - both the documented code facts
# and the current understanding of the system. Wired in ..\settings.json.
# Receives the hook input JSON on stdin; locates its own repo via $PSScriptRoot.
$raw = [Console]::In.ReadToEnd()
try { $in = $raw | ConvertFrom-Json } catch { exit 0 }
$f = $in.tool_input.file_path
if (-not $f) { $f = $in.tool_response.filePath }
if (-not $f) { exit 0 }
$repo = (Resolve-Path (Join-Path $PSScriptRoot '..\..')).Path
try { $full = (Resolve-Path $f -ErrorAction Stop).Path } catch { $full = $f }
if (-not $full.StartsWith($repo, [System.StringComparison]::OrdinalIgnoreCase)) { exit 0 }
if ($full -match '\\(bin|obj|packages|\.vs|\.git|\.claude)\\') { exit 0 }
if ($full -match '\\CLAUDE\.md$') { exit 0 }
if ($full -notmatch '\.(cs|config|csproj|sln|xml)$') { exit 0 }
$disc = ($repo -replace '[^A-Za-z0-9]','')
if ($disc.Length -gt 40) { $disc = $disc.Substring($disc.Length - 40) }
$marker = Join-Path $env:TEMP ('claudemd-' + $in.session_id + '-' + $disc)
if (Test-Path $marker) { exit 0 }
New-Item -ItemType File -Path $marker -Force | Out-Null
$msg = "You changed source in this project. Before finishing this task, check this repo's " +
       "CLAUDE.md (at the repo root) on two fronts: (1) FACTS - does the change alter anything " +
       "it documents (solution/namespace, host/client/operation/service/collection lists, " +
       "config variants, structure, noted quirks)? (2) UNDERSTANDING - has what you've learned " +
       "this session evolved your understanding of this system's purpose, behavior, or quirks " +
       "beyond what CLAUDE.md currently says, even independently of this edit? If either " +
       "applies, update CLAUDE.md in the same task. Routine edits that change no documented " +
       "fact and no understanding need no update."
@{ hookSpecificOutput = @{ hookEventName = 'PostToolUse'; additionalContext = $msg } } | ConvertTo-Json -Compress
exit 0
