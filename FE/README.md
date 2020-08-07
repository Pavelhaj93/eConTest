# eContracting

Based on combination of React, Bootstrap v4.x, React Bootstrap and Typescript ❤️

## Setup

1. Install [Node.js](https://nodejs.org).
2. Install [Yarn](https://yarnpkg.com).

## Installation

`yarn install`

## Usage

### Development

`yarn start`

### Build

`yarn build`

The build command will generate all the static assets needed for BE guys.
Once the build is complete, all the genereted files are automatically copied to the specified directory, so BE guys can actually use them and deploy them to test/prod environment.

### Want to create a new page?

1. Create a new file `page-name.handlebars` inside `src/tpl/` directory.

### Want to add a new icon to SVG sprite?

1. Place the SVG file inside `src/icons/svg/` directory.
2. Import the new SVG icon at the beginning of `src/app/app.tsx`.

After build, the icon will be automatically part of SVG sprite.
