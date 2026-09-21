# Contributing to AreaCut

## Bug Reports

Open an issue with:
- AreaCut version
- Windows version
- Steps to reproduce
- Expected vs actual behavior
- Log output (if available)

## Code Style

- C# with `#nullable enable`
- Meaningful XML doc comments on public APIs
- Small methods, clear ownership
- No unnecessary abstractions
- Async where appropriate, with `CancellationToken`
- `IDisposable` / COM cleanup on all native resources

## Commit Messages

Use conventional commits:

```
feat(media): add Media Foundation video probing
feat(timeline): implement non-destructive clip trimming
fix(export): correct audio sync offset
docs(architecture): update dependency diagram
test(core): add TimeStamp round-trip tests
```

## Pull Requests

- One logical change per PR
- Include tests
- Update documentation
- Verify `dotnet build` and `dotnet test` pass

## License

By contributing, you agree that your contributions will be licensed under the MIT License.
