Changelog
=========

All notable changes to this project will be documented in this file.

The format is inspired by Keep a Changelog, and this project adheres to Semantic Versioning where applicable.

## [Unreleased]

### Added
- Reflection-free, AOT/NativeAOT-friendly JSON by generating per-type converters for `[XJson]` models.
- `Neemle.XJson.Generated.Json` helper with `Encode<T>`, `Decode<T>`, and `Validate<T>` APIs.
- Overloads that accept `JsonSerializerOptions?` to control naming and null handling at runtime.
- Overloads that accept `JsonSourceGenerationOptionsAttribute?` as a convenient options bag, internally mapped to `JsonSerializerOptions`.
- Support for `[JsonPropertyName]` on properties; attribute names take precedence over policies.
- Mapping from `JsonKnownNamingPolicy` to `JsonNamingPolicy` when available on target frameworks (e.g., Snake/Kebab on .NET 8+, TrainCase on .NET 9+).
- Project README with quick start, configuration, supported types, limitations, and AOT notes.

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
