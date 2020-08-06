interface IJsonResponse {
  [index: string]: any
}

interface Params {
  [index: string]: string | undefined
}

const headers = {
  'Content-Type': 'application/json',
  Accept: 'application/json',
}

const checkResponse = (response: Response): Response => {
  if (response.ok) {
    return response
  }

  throw new Error(`${response.status} ${response.statusText}`)
}

const parseJSON = (response: Response): Promise<IJsonResponse> => {
  // if response is 201 (created), then don't parse the response since there is no json
  if (response.status === 201) {
    return new Promise(resolve => resolve())
  }

  return response.json().then(json => JSON.parse(json.d))
}

export const simpleFetch = {
  /**
   * Sends a POST request to given URL.
   * @param url - string
   * @param params - object contains parameters for the request
   * @returns Promise with <IJsonResponse> shape
   */
  post: (url: RequestInfo, params: Params): Promise<IJsonResponse> => {
    return fetch(url, {
      method: 'POST',
      headers,
      body: JSON.stringify(params),
    })
      .then(checkResponse)
      .then(parseJSON)
  },
}
