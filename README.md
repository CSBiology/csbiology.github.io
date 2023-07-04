# csbiology.github.io
The csb website.

# Local Development

## Setup

1. `dotnet tool restore`
2. `npm install`
3. Add olat password as `src/loaders/olat.p`, containing only the password as raw string.

# Update css from scss

- `npm run update-css`, will create minified css from `src/style/scss/main.scss`.

## Run

1. `npm run fornax`
2. Go to [http://127.0.0.1:8080](http://127.0.0.1:8080).