import React, { useEffect, useState } from 'react'

type Props = {
  src: string
  alt: string
  className?: string
}

export const PreloadImage: React.FC<Props> = ({ src, alt, className }) => {
  const [loaded, setLoaded] = useState(false)

  useEffect(() => {
    const image = new Image()

    image.onload = () => {
      setLoaded(true)
    }

    image.src = src
  }, [src])

  if (loaded) {
    return <img src={src} alt={alt} className={className} />
  }

  return (
    <div className="loading loading--image">
      {/* https://css-tricks.com/preventing-content-reflow-from-lazy-loaded-images/#we-have-a-winner */}
      <img
        src="data:image/svg+xml,%3Csvg xmlns='http://www.w3.org/2000/svg' viewBox='0 0 3 2'%3E%3C/svg%3E"
        alt={alt}
        className={className}
      />
    </div>
  )
}
