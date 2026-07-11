# Smaoin.Shared

Small, well-scoped **ASP.NET Core** building blocks — the reusable pieces behind [Smaoin's](https://smaoin.co.uk) client work, published as independent NuGet packages so you take only what you need.

[![License: MIT](https://img.shields.io/badge/License-MIT-c14600.svg)](LICENSE)
[![.NET](https://img.shields.io/badge/.NET-10.0-512bd4.svg)](https://dotnet.microsoft.com)

Every package exposes a single `AddSmaoin*()` (or `UseSmaoin*()`) extension that binds its options from configuration, validates required fields, and fails fast at startup if anything is missing.

## Packages

| Package | NuGet | What it does |
|---|---|---|
| **Smaoin.Shared.Security** | [![NuGet](https://img.shields.io/nuget/v/Smaoin.Shared.Security.svg)](https://www.nuget.org/packages/Smaoin.Shared.Security) | CSP with per-request nonce, security headers, `__Host-` antiforgery, a non-production robots blocker, and a proxy-aware client-IP resolver |
| **Smaoin.Shared.Captcha** | [![NuGet](https://img.shields.io/nuget/v/Smaoin.Shared.Captcha.svg)](https://www.nuget.org/packages/Smaoin.Shared.Captcha) | Cloudflare Turnstile verification with hostname/action binding, a hard HTTP timeout, and fail-closed behaviour |
| **Smaoin.Shared.Seo** | [![NuGet](https://img.shields.io/nuget/v/Smaoin.Shared.Seo.svg)](https://www.nuget.org/packages/Smaoin.Shared.Seo) | SEO meta tags, JSON-LD structured data (Organization / Breadcrumb), and XML sitemap generation |
| **Smaoin.Shared.Cdn** | [![NuGet](https://img.shields.io/nuget/v/Smaoin.Shared.Cdn.svg)](https://www.nuget.org/packages/Smaoin.Shared.Cdn) | Fully-qualified, content-hash cache-busted CDN asset URLs |

## Install

```bash
dotnet add package Smaoin.Shared.Security
```

## Quick start

```csharp
// Program.cs
builder.Services.AddSmaoinSecurityHeaders();   // binds the "SecurityHeaders" config section
builder.Services.AddSmaoinTurnstile();          // binds "Turnstile"

var app = builder.Build();
app.UseSmaoinSecurityHeaders();                 // emits CSP + security headers per request
```

```jsonc
// appsettings.json
{
  "SecurityHeaders": {
    "ContentSecurityPolicy": "default-src 'self'; script-src 'self' 'nonce-{nonce}'"
  },
  "Turnstile": {
    "SiteKey": "…",
    "SecretKey": "…",
    "ExpectedHostnames": [ "example.com" ]
  }
}
```

Each `*Options` class exposes a `public const string SectionName` — reference it rather than hardcoding the section name.

## Requirements

- .NET 10 SDK

## About

Built and maintained by [**Smaoin Ltd**](https://smaoin.co.uk) — bespoke business systems, integrations, and secure websites for SMEs. Elgin, Scotland.

> **Note:** development happens in Smaoin's internal repository; this repo is the public home for the open-source packages. Issues and feedback are welcome here; pull requests may be ported upstream.

## License

[MIT](LICENSE) © Smaoin Ltd
