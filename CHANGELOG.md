Changelog
=========

All notable changes to this project will be documented in this file.

The format is inspired by Keep a Changelog, and this project adheres to Semantic Versioning where applicable.

## [Unreleased]

### Added
- Nothing yet.

## [1.0.2] - 2025-11-24

### Added
- Design-time stub for `Neemle.XJson.Generated.Json` that surfaces the API in IDEs when no `[XJson]` types are present, preventing red squiggles without enabling reflection at runtime.

### Changed
- Generator now emits a typed throw-only helper at design time instead of a `JsonSerializer`-based fallback, keeping runtime paths reflection-free.

### Changed
- Removed reliance on `JsonSerializerContext` and STJ’s source generator; the XJson generator now emits its own converters and dispatch logic.

### Notes
- Current supported types: `string`, `bool`, `int`, `long`, and their nullable variants; nested `[XJson]` types.
- Limitations: no enums, collections, floating-point/decimal, Guid, DateTime/Offset, polymorphism, or custom converters yet.
## [1.0.1.0] - 2025-11-21

### Added
- Support for different JSON value types.

### Notes
- Maintenance release to align version metadata and prepare for publishing.

## [1.0.0.0] – Initial release
- Initial public release of XJson Abstractions, Generator, and SampleApp.
