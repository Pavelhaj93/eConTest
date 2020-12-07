import React from 'react'
import { render, screen, waitFor } from '@testing-library/react'
import fetch from 'jest-fetch-mock'
import { AcceptedOffer } from '../'

const mockProps = {
  view: 'Offer',
  offerUrl: '',
  errorPageUrl: '',
  thankYouPageUrl: '',
  getFileUrl: '/',
  getFileForSignUrl: '',
  signFileUrl: '',
  labels: {},
}

const acceptedOfferResponse = {
  groups: [
    {
      title: 'Dodatek a přidružené dokumenty',
      files: [
        {
          label: 'Informace pro zákaznika.pdf',
          key: 'a3f7c50c605da4a8d9493fd456d71b032',
          mime: 'application/pdf',
          size: '53 KB',
        },
      ],
    },
  ],
}

describe('Accepted offer view', () => {
  it('renders error message when API is unavailable', async () => {
    fetch.mockRejectOnce(() => Promise.reject('API is unavailable'))

    render(<AcceptedOffer {...mockProps} />)

    const errorMessage = await waitFor(() => screen.getByRole('alert'))
    expect(errorMessage).toBeInTheDocument()
  })

  it('renders group heading', async () => {
    fetch.mockResponseOnce(JSON.stringify(acceptedOfferResponse))

    render(<AcceptedOffer {...mockProps} />)

    const heading = await waitFor(() => screen.getByText(acceptedOfferResponse.groups[0].title))
    expect(heading).toBeInTheDocument()
  })
})
