# Supported formats

AreaCut decodes through **Windows Media Foundation**, so what you can import
depends on the codecs installed on your machine rather than on bundled
libraries. The tables below are the target support matrix.

> **Not importable yet.** The import pipeline is currently a stub — the
> `MediaProber` and reader types exist but do not decode anything. Today nothing
> can actually be brought into a project. See the
> [roadmap](../development/roadmap.md).

## Video

| Format | Container | Video | Audio | Notes |
| --- | --- | --- | --- | --- |
| MP4 | ✅ | H.264 | AAC | Primary format, produced by AreaRec |
| M4V | ✅ | H.264 | AAC | |
| MOV | ⚠️ | H.264 | AAC | Depends on the installed Media Foundation codecs |
| AVI | ⚠️ | Varies | Varies | Limited; older codecs often missing |
| WMV | ⚠️ | WMV | WMA | Windows-only legacy |

## Audio

| Format | Codec | Notes |
| --- | --- | --- |
| MP3 | MP3 | Audio-only clips |
| WAV | PCM | Audio-only clips |
| M4A | AAC | Audio-only clips |

## Images

| Format | Notes |
| --- | --- |
| PNG | Image clip, still duration |
| JPEG | Image clip, still duration |
| WebP | ⚠️ Depends on the installed codecs |

## What happens with an unsupported file

AreaCut checks Media Foundation capabilities and reports a clear error instead
of failing silently or crashing. Formats marked ⚠️ may or may not work on a
given Windows installation — that is a property of the system, not a bug.

## Relinking moved files

Projects store **paths**, not copies of your media. If a source file moves,
AreaCut marks the reference offline, searches the project folder and common
locations, and offers a relink dialog. Your original files are never modified or
imported into the project.
