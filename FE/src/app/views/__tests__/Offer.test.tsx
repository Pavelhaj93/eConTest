import React from 'react'
import { render, screen, waitFor } from '@testing-library/react'
import fetch from 'jest-fetch-mock'
import userEvent from '@testing-library/user-event'
import { Offer } from '../'

const mockProps = {
  guid: '0635F899B3111EDC9BC4A505A628FCED',
  view: 'Offer',
  offerUrl: '',
  errorPageUrl: '',
  thankYouPageUrl: '',
  sessionExpiredPageUrl: '',
  getFileUrl: '',
  getFileForSignUrl: '',
  signFileUrl: '',
  labels: {
    signatureBtn: 'Podepsat',
    submitBtn: 'Akceptuji',
  },
}

const basicOfferResponse = {
  perex: {
    title: 'Stručný přehled Vaší nabídky',
    params: [
      {
        title: 'Produkt',
        value: 'Relax',
      },
      {
        title: 'Platnost nabídky do',
        value: '24 měsíců',
      },
      {
        title: 'Délka fixace',
        value: '18.09.2020',
      },
      {
        title: 'Účinnost smlouvy',
        value: '01.10.2020',
      },
    ],
  },
  documents: {
    acceptance: {
      title: 'Dokumenty k elektronické akceptaci Vaši smlouvy',
      text:
        '<p>Va&scaron;e adresa: {PERSON_PREMADR}<br />\n{PERSON_PREMLABEL}: {PERSON_PREMEXT}</p>',
      accept: {
        title: 'Dodatek a přidružené dokumenty',
        subTitle: '<p>Dokument(y) si přečtěte a potvrďte zatržen&iacute;m</p>',
        mandatoryGroups: ['06D969E88C3C1EDB8BD27598F0B3C7D8'],
        params: [
          {
            title: 'Vaše adresa',
            value: 'Ledčice 178, 277 08 Ledčice',
          },
          {
            title: 'EAN',
            value: 'RRI329212',
          },
        ],
        files: [
          {
            label: 'Informace pro zákazníka - spotřebitele',
            prefix: 'Souhlasím s',
            key: 'e1c9a5ce583743e6928ee4df91862a91',
            group: '06D969E88C3C1EDB8BD27598F0B3C7D8',
          },
          {
            label: 'Dodatek',
            prefix: 'Jsem poučen o',
            key: '8e0ed4754f99464eb2d0155c140a2541',
            group: '06D969E88C3C1EDB8BD27598F0B3C7D8',
          },
        ],
      },
      sign: {
        title: 'Plná moc',
        subTitle: 'Plnou moc potřebujeme pro zajištění pokračování dodávky u innogy',
        mandatoryGroups: ['06D969E88C3C1EDB8BD27637116827D8'],
        files: [
          {
            label: 'Plná moc',
            prefix: 'Jsem poučen o',
            key: '5e40683abb1441f5a0ec99425c2c6908',
            mandatory: true,
            group: '06D969E88C3C1EDB8BD27637116827D8',
          },
        ],
        note: 'Podepište snadno myší přímo ve Vašem prohlížeči nebo prstem v tabletu',
      },
    },
  },
}

const singleGiftResponse = {
  gifts: {
    title: 'Máme pro Vás nachystané dárky',
    note: 'Způsob doručení korespondenčně na zasílací adresu',
    groups: [
      {
        title: 'Za čas věnovaný naší nabídce od nás jako pozornost získáváte:',
        params: [
          {
            title: '100x LED žárovka',
            icon: 'LED',
            count: 100,
          },
          {
            title: '20x detektor',
            icon: 'DET',
            count: 20,
          },
          {
            title: '1x poukázka Kaufland 10 Kč',
            icon: 'PKZ',
            count: 1,
          },
        ],
      },
    ],
  },
  documents: {},
}

const multipleGiftsResponse = {
  gifts: {
    title: 'Máme pro Vás nachystané dárky',
    note: 'Způsob doručení korespondenčně na zasílací adresu',
    groups: [
      {
        title: 'Za čas věnovaný naší nabídce od nás jako pozornost získáváte:',
        params: [
          {
            title: '100x LED žárovka',
            icon: 'LED',
            count: 100,
          },
          {
            title: '20x detektor',
            icon: 'DET',
            count: 20,
          },
          {
            title: '1x poukázka Kaufland 10 Kč',
            icon: 'PKZ',
            count: 1,
          },
        ],
      },
      {
        title: 'Za přijetí nabídky za Vás čeká:',
        params: [
          {
            title: '10x LED žárovka',
            icon: 'LED',
            count: 10,
          },
          {
            title: '20x detektor',
            icon: 'DET',
            count: 20,
          },
          {
            title: '1230 x poukázka Hornbach, 1000 Kč na vybavení zahrady',
            icon: 'PKZ',
            count: 1230,
          },
        ],
      },
    ],
  },
  documents: {},
}

describe('Offer view', () => {
  it('renders offer summary', async () => {
    fetch.mockResponseOnce(JSON.stringify(basicOfferResponse))

    render(<Offer {...mockProps} />)

    const table = await waitFor(() => screen.getByTestId('summaryTable'))
    expect(table).toBeInTheDocument()
  })

  it('renders checkbox for each document received for acceptance', async () => {
    fetch.mockResponseOnce(JSON.stringify(basicOfferResponse))

    render(<Offer {...mockProps} />)

    const container = await waitFor(() => screen.getByTestId('boxDocumentsToBeAccepted'))
    const checkboxes = Array.from(container.querySelectorAll('input[type="checkbox"]'))

    expect(checkboxes.length).toBe(basicOfferResponse.documents.acceptance.accept.files.length)
  })

  it('renders gift box with shared title', async () => {
    fetch.mockResponseOnce(JSON.stringify(multipleGiftsResponse))

    render(<Offer {...mockProps} />)

    const giftBoxHeading = await waitFor(() => screen.getByTestId('giftBoxHeading'))
    expect(giftBoxHeading).toHaveTextContent(multipleGiftsResponse.gifts.title)
  })

  it('renders gift box with title from single gift group', async () => {
    fetch.mockResponseOnce(JSON.stringify(singleGiftResponse))

    render(<Offer {...mockProps} />)

    const giftBoxHeading = await waitFor(() => screen.getByTestId('giftBoxHeading'))
    expect(giftBoxHeading).toHaveTextContent(singleGiftResponse.gifts.groups[0].title)
  })

  it('renders "Sign" button for each document recevied for signature', async () => {
    fetch.mockResponseOnce(JSON.stringify(basicOfferResponse))

    render(<Offer {...mockProps} />)

    const buttons = await waitFor(() => screen.getAllByText(mockProps.labels.signatureBtn))
    expect(buttons.length).toBe(basicOfferResponse.documents.acceptance.sign.files.length)
  })

  it('opens sign modal', async () => {
    fetch.mockResponseOnce(JSON.stringify(basicOfferResponse))

    render(<Offer {...mockProps} />)

    const signBtn = await waitFor(() => screen.getByText(mockProps.labels.signatureBtn))
    userEvent.click(signBtn)

    const modal = await waitFor(() => screen.getByRole('dialog'))
    expect(modal).toBeInTheDocument()
  })

  it.skip('allows to accept the offer', async () => {
    const offerResponse = {
      documents: {
        acceptance: {
          accept: {
            mandatoryGroups: ['group1'],
            files: [
              {
                label: 'Informace pro zákazníka - spotřebitele',
                prefix: 'Souhlasím s',
                key: 'e1c9a5ce583743e6928ee4df91862a91',
                group: 'group1',
              },
              {
                label: 'Dodatek',
                prefix: 'Jsem poučen o',
                key: '8e0ed4754f99464eb2d0155c140a2541',
                group: 'group1',
              },
            ],
          },
        },
      },
    }

    fetch.mockResponseOnce(JSON.stringify(offerResponse))

    render(<Offer {...mockProps} />)

    const checkboxes = await waitFor(() => screen.getAllByRole('checkbox'))
    checkboxes.forEach(checkbox => userEvent.click(checkbox))

    const acceptBtn = await waitFor(() => screen.getByText(mockProps.labels.submitBtn))
    expect(acceptBtn).not.toBeDisabled()
  })

  it('renders general error message when API is unavailable', async () => {
    fetch.mockRejectOnce(() => Promise.reject('API is unavailable'))

    render(<Offer {...mockProps} />)

    const alert = await waitFor(() => screen.getByRole('alert'))
    expect(alert).toBeInTheDocument()
  })

  it.skip('renders custom error message from API', async () => {
    const errorMessage = 'Offer is not ready.'

    fetch.mockResponseOnce(JSON.stringify({ Message: errorMessage }), {
      headers: { 'content-type': 'application/json' },
      status: 500,
    })

    render(<Offer {...mockProps} />)

    const errorMessageEl = await waitFor(() => screen.getByTestId('errorMessage'))
    expect(errorMessageEl).toHaveTextContent(errorMessage)
  })
})
