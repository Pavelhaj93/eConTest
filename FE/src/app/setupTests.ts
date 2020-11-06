import fetchMock from 'jest-fetch-mock'

fetchMock.enableMocks()

// turn of `console.error` message
global.console = {
  ...console,
  error: jest.fn(),
}
