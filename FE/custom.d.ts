declare module '*.svg'
declare module 'react-signature-pad-wrapper'

type DataURL = 'image/jpeg' | 'image/svg+xml'

/**
 * Custom types definition for SignaturePad.
 * https://github.com/szimek/signature_pad#api
 */
interface SignaturePad {
  toDataURL: (type?: DataURL) => string
  clear: () => void
  isEmpty: () => boolean
  off: () => void
  on: () => void
}
