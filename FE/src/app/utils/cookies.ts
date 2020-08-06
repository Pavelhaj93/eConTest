/**
 * Set cookie.
 * @param cname - cookie name
 * @param cvalue - cookie value
 * @param exdays - a number, after how many days the cookie will expire
 */
export const setCookie = (cname: string, cvalue: string, exdays: number): void => {
  const d = new Date()
  d.setTime(d.getTime() + exdays * 24 * 60 * 60 * 1000)

  const expires = `expires=${d.toUTCString()}`
  document.cookie = `${cname}=${cvalue};${expires};path=/`
}

/**
 * Delete a cookie.
 * @param cname - cookie name
 */
export const deleteCookie = (cname: string): void => {
  document.cookie = `${cname}=; expires=Thu, 01 Jan 1970 00:00:00 UTC;path=/;`
}
