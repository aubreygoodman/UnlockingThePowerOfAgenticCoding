# Sample Page for Link Checker Exercise

This file contains a mix of valid and broken links for testing the Markdown link checker.

## Valid links

- [Google](https://www.google.com)
- [GitHub](https://github.com)
- [Microsoft .NET](https://dotnet.microsoft.com)

## Likely broken links

- [Broken Example](https://thisdomaindoesnotexist12345.com)
- [Bad Path](https://www.google.com/this-page-does-not-exist-404)

## Edge cases

- [Empty URL]()
- [Relative link](./other-file.md)
- [Anchor link](#valid-links)
- [URL with query string](https://www.google.com/search?q=test)
- [URL with fragment](https://github.com#about)

## Inline links

Here is a [valid inline link](https://www.google.com) in the middle of text,
and here is a [broken inline link](https://httpstat.us/500) that returns a 500.

Multiple links on one line: [one](https://www.google.com) and [two](https://github.com) and [three](https://thisdomaindoesnotexist99999.com).
