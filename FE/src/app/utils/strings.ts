import { CommodityProductType, QueryParams } from '@types'

export const generateId = (): string => Math.random().toString(36).slice(2)

export const getQueryString = (params: QueryParams): string =>
  Object.keys(params)
    .filter(key => params[key] !== null && params[key] !== undefined)
    .map(key => `${encodeURIComponent(key)}=${encodeURIComponent(params[key])}`)
    .join('&')

export class RelativeURL {
  public origin = ''
  public pathname: string
  public search: string

  constructor(url: string) {
    this.pathname = this.getPathname(url)
    this.search = this.getSearch(url)
  }

  private getPathname = (url: string) => {
    const searchIndex = url.indexOf('?')
    return searchIndex !== -1 ? url.slice(0, searchIndex) : url
  }

  private getSearch = (url: string) => {
    const searchIndex = url.indexOf('?')
    return searchIndex !== -1 ? url.slice(searchIndex) : ''
  }
}

/**
 *  Example of returned value: `"www.example.com/api?param1=value1&param2=value2"`
 * @returns Encoded parsed url
 */
export const parseUrl = (url: string, params: QueryParams): string => {
  // Regular expression that matches strings, that are absolute urls.
  const r = new RegExp('^(?:[a-z]+:)?//', 'i')
  const isAbsoluteUrl = r.test(url)

  const { origin, pathname, search } = isAbsoluteUrl ? new URL(url) : new RelativeURL(url)

  const queryString = getQueryString(params)

  return `${origin}${pathname === '/' ? '' : pathname}${search ? search + '&' : '?'}${queryString}`
}

/* Remove last element from array of strings. */
export const removeLastElement = (array: string[]): string | undefined => array.pop()

export const getColorByCommodityType = (commodityType: CommodityProductType): string => {
  switch (commodityType) {
    case CommodityProductType.ELECTRICITY:
      return 'purple-light'
    case CommodityProductType.GAS:
      return 'blue'
    default:
      return 'purple-light'
  }
}
