# csbiology.github.io

The official [CSB-Website](https://csbiology.github.io).


# Local Development

## Setup

1. `dotnet tool restore`
2. `npm install`
3. Add olat password as `src/loaders/olat.p`, containing only the password as raw string.

## Update css from scss

- `npm run update-css`, will create minified css from `src/style/scss/main.scss`.

## Run

1. `npm run fornax`
2. Go to [http://127.0.0.1:8080](http://127.0.0.1:8080).

## Site Config Information:

### Team Members

*Using myself as an example*

Create a `.md` file with your name in snake_case in `src/content/team`. All fields not marked with an `#!` are optional.

```yaml
# src/content/team/kevin_frey.md
---
name: Kevin Frey #!
img: frey.png
role: PhD Student #!
github: Freymaurer
orcid: 0000-0002-8510-6810
alumni: 2020-Master Student
phone: +49 000 111 4242 # this is only an example 
email: freyk@rptu.de
---
```