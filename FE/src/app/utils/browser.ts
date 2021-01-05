// https://developer.mozilla.org/en-US/docs/Web/API/Window/crypto
export const isIE11 = (): boolean => Boolean(window.msCrypto)
