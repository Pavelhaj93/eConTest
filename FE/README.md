# eContracting

Based on combination of React, Bootstrap v4.x, React Bootstrap and Typescript ❤️\
Some components are using MobX for local state management.\
For maximum compatibility with IE11, older version of MobX is used.

## Setup

1. Install [Node.js](https://nodejs.org) (v12.14.1 or higher).
2. Install [Yarn](https://yarnpkg.com).

## Installation

`yarn install`

## Usage

### Development

`yarn start`

### Build

`yarn build`

The build command will generate all the static assets needed for BE guys.\
Once the build is complete, all the genereted files are automatically copied to the specified directory, so BE guys can actually use them and deploy them to test/prod environment.

### Want to create a new page?

1. Create a new file `page-name.handlebars` inside `src/tpl/` directory.

### Want to add a new icon to SVG sprite?

1. Place the SVG file inside `src/icons/svg/` directory.
2. Run `yarn svg`.
3. Use it as a component e.g. `<Icon name="arrow-right" width="10" height="20" color="orange" />`

After build, the icon will be automatically part of SVG sprite.

### Mock API

API is mocked using [connect-api-mocker](https://github.com/muratcorlu/connect-api-mocker) library.\
Implementation of all mocked APIs and mock data can be found inside `src/mocks/api` directory or check the description below.

#### `GET` /api/offer/accepted

Returns JSON in shape of `AcceptedOfferResponse`. Check `@types/Offer.ts` for more details.

#### `GET` /api/offer/new

Returns JSON in shape of `NewOfferResponse`. Check `@types/Offer.ts` for more details.

#### `GET` /api/document-image/{key}

Returns PNG image.

#### `GET` /api/summary

Returns JSON in shape of `SummaryResponse`. Check `@types/Summary.ts` for more details.

#### `POST` /api/upload-document/{groupId}

Request body params:

- `file` - an actual file
- `key` - unique key of the uploading file

Params need to be sent as `FormData`.

Returns JSON in shape of `UploadDocumentResponse`. HTTP status code `200` is returned when upload is successful, `400` when fails.

```json
{
  "id": "4310a9b78dbb4b2fbf77c1502f420ecb", // group identifier
  "size": 64578, // total size of all uploaded files in the current group in bytes
  // all the uploaded files in the current group including the newest one that was uploaded
  "files": [
    {
      "label": "IMG_3422.jpg",
      "key": "89a5219bc72",
      "mime": "image/jpeg",
      "size": 12235
    },
    {
      "label": "smlouva.pdf",
      "key": "6fec15d2e98",
      "mime": "application/pdf",
      "size": 52343
    }
  ]
}
```

#### `DELETE` /api/remove-document/{groupId}

Query string params:

- `f` - unique key of the file

Returns JSON in shape of `UploadDocumentResponse`. HTTP status code `200` is returned when removal of the file is successful, `400` when fails.

#### `POST` /api/sign-document/{key}

Request body params:

- `signature` - PNG representation of signature encoded as base64 (string)

HTTP status code `200` is returned when sign process is successful, `400` when fails.

#### `POST` /api/offer/accept

Request body JSON:

```json
{
  "accepted": [], // array of keys
  "signed": [], // array of keys
  "uploaded": [], // array of keys
  "other": [] // array of keys
}
```

HTTP status code `200` is returned when offer is accepted, `400` when fails.

## Other information

The header and footer parts are rendered directly by Sitecore.
