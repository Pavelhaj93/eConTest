import { parseUrl } from './strings'

describe('parseUrl function', () => {
  it('should return correct url from absolute url ending by /', () => {
    const url = parseUrl('http://example.com/', { param: 'value' })

    expect(url).toBe('http://example.com?param=value')
  })
  it('should return correct url from absolute url', () => {
    const url = parseUrl('http://example.com', { param: 'value' })

    expect(url).toBe('http://example.com?param=value')
  })
  it('should return correct url from absolute url with existing search parameters', () => {
    const url = parseUrl('http://example.com?text=hello', { param: 'value' })

    expect(url).toBe('http://example.com?text=hello&param=value')
  })
  it('should return correct url from relative url with param', () => {
    const url = parseUrl('path/name', { param: 'value' })

    expect(url).toBe('path/name?param=value')
  })
  it('should return correct url from relative url', () => {
    const url = parseUrl('path/name?text=hello', { param: 'value' })

    expect(url).toBe('path/name?text=hello&param=value')
  })
})
