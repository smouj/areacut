# Keyboard and mouse reference

The WinUI 3 shell binds the shortcuts below. Because the interface is still a
shell, most of them act on the engine or the preview clock but produce **no
visible result yet** — the `State` column says what each one really does today.

## Playback

| Key | Action | State |
| --- | --- | --- |
| `Space` | Play / pause | Drives the preview clock. Nothing is rendered yet. |
| `K` | Pause | Drives the preview clock. |
| `J` | Reverse playback | ⛔ Bound but empty — does nothing. |
| `L` | Forward playback | ⛔ Bound but empty — does nothing. |
| `←` | Step back one frame (1/30 s) | Moves the clock. |
| `→` | Step forward one frame (1/30 s) | Moves the clock. |

## Editing

| Key | Action | State |
| --- | --- | --- |
| `S` | Split the selected clip at the playhead | 🚧 Requires a selected clip; clip selection is not wired to the UI yet, so it cannot fire. |
| `Delete` | Delete the selected clip | 🚧 Same limitation as `S`. |
| `Ctrl+Z` | Undo | ✅ Calls the undo stack. |
| `Ctrl+Y` | Redo | ✅ Calls the redo stack. |
| `Ctrl+S` | Save project | ⛔ Bound but empty — saving from the UI is not implemented. |

## Timeline (planned)

These behaviours are specified in [../development/timeline.md](../development/timeline.md)
and are **not implemented** — there is no functional timeline in the app yet.

| Input | Action |
| --- | --- |
| Click | Select clip |
| Drag clip | Move, with snapping |
| Drag left / right edge | Trim in / out point |
| Scroll wheel | Zoom horizontally |
| `Shift` + scroll | Pan horizontally |
| `Ctrl+D` | Duplicate clip |

## Editor conventions

- **Non-destructive:** every one of these operations changes project state only.
  Your source files are never touched.
- **Undo covers everything:** each edit is a command on the undo stack, and a new
  edit clears the redo stack.
