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
  data: [
    {
      type: 'perex',
      position: 1,
      header: {
        title: 'Stručný přehled Vaší nabídky',
      },
      body: {
        params: [
          {
            title: 'Produkt',
            value: 'plyn Start 12',
          },
          {
            title: 'Platnost nabídky do',
            value: '05.10.2022',
          },
          {
            title: 'Délka fixace',
            value: '12 měsíců od účinnosti smlouvy',
          },
          {
            title: 'Platnost smlouvy',
            value: 'Dnem akceptace smlouvy zákazníkem',
          },
          {
            title: 'Předpokládaná účinnost smlouvy',
            value: '09.10.2022',
          },
        ],
      },
    },
    {
      type: 'docsCheck',
      position: 3,
      header: {
        title: '',
      },
      body: {
        head: {
          title: 'Dokumenty k elektronické akceptaci Vaší smlouvy',
          params: [
            {
              title: 'Na adrese místa spotřeby',
              value: 'Sobotní 736, 691 42 Valtice',
            },
            {
              title: 'EIC',
              value: '27ZG600Z0414883V',
            },
          ],
          text:
            '<p>Přečtěte si pros&iacute;m v&scaron;echny dokumenty a zatržen&iacute;m potvrďte jejich přečten&iacute;. Nakonec klikněte na tlač&iacute;tko Akceptuji.</p>\n<p>Posl&eacute;ze budou v&scaron;echny dokumenty odesl&aacute;ny do innogy a nen&iacute; třeba je d&aacute;le zas&iacute;lat jinou formou.</p>',
        },
        text: null,
        docs: {
          title: 'Smlouva a přidružené dokumenty',
          params: null,
          text: null,
          mandatoryGroups: ['0635F899B3111EED8E9B07EF632D3EC9'],
          files: [
            {
              key: '9D84515F7BD67F987011878FD0E9DCB1',
              group: '0635F899B3111EED8E9B07EF632D3EC9',
              label: 'Informace pro zákazníka – spotřebitele',
              note: null,
              prefix: 'Souhlasím s',
              mime: 'application/pdf',
              mandatory: true,
              idx: 1,
            },
            {
              key: '1588852AACD6AABB2795488FF6039B8C',
              group: '0635F899B3111EED8E9B07EF632D3EC9',
              label: 'Smlouva o sdružených službách dodávky plynu',
              note: null,
              prefix: 'Jsem poučen o',
              mime: 'application/pdf',
              mandatory: true,
              idx: 2,
            },
            {
              key: '1DD58C9CD8109A5AABE3415E01FF4842',
              group: '0635F899B3111EED8E9B07EF632D3EC9',
              label: 'Ceník',
              note: null,
              prefix: 'Jsem poučen o',
              mime: 'application/pdf',
              mandatory: true,
              idx: 4,
            },
            {
              key: 'F48CE2AEF4ED1C52D42504F68F66DD29',
              group: '0635F899B3111EED8E9B07EF632D3EC9',
              label: 'Ceník služeb',
              note: null,
              prefix: 'Jsem poučen o',
              mime: 'application/pdf',
              mandatory: true,
              idx: 5,
            },
            {
              key: '4B1770D545BD73151EA656AA0FD1B1D9',
              group: '0635F899B3111EED8E9B07EF632D3EC9',
              label: 'Obchodní podmínky',
              note: null,
              prefix: 'Jsem poučen o',
              mime: 'application/pdf',
              mandatory: true,
              idx: 6,
            },
            {
              key: 'EFEB578CC0241BBEEAA3CC78D4FD5BC5',
              group: '0635F899B3111EED8E9B07EF632D3EC9',
              label: 'Potvrzení o přiznání individuální slevy',
              note: '',
              prefix: 'Jsem poučen o',
              mime: 'application/pdf',
              mandatory: false,
              idx: 17,
            },
          ],
        },
        note: null,
      },
    },
    {
      type: 'docsSign',
      position: 4,
      header: {
        title: 'Dokumenty k podpisu',
      },
      body: {
        head: null,
        text: null,
        docs: {
          title: 'Plná moc',
          params: null,
          text:
            '<p>Plnou moc potřebujeme pro zah&aacute;jen&iacute; dod&aacute;vky u innogy.&nbsp;</p>',
          mandatoryGroups: ['0635F899B3111EED8E9B07EF632D3EC9'],
          files: [
            {
              key: 'EEFCB55B021DA472D361BD2E894BBDD9',
              group: '0635F899B3111EED8E9B07EF632D3EC9',
              label: 'Plná moc',
              note: '',
              prefix: 'Jsem poučen o',
              mime: 'application/pdf',
              mandatory: true,
              idx: 3,
            },
          ],
        },
        note: 'Podepište snadno myší přímo ve Vašem prohlížeči nebo prstem v tabletu',
      },
    },
    {
      type: 'docsCheck',
      position: 5,
      header: {
        title: 'Doplňkové služby',
      },
      body: {
        head: null,
        text: null,
        docs: {
          title: 'Dokument(y) k elektronické akceptaci služby',
          params: null,
          text:
            '<p>Nunc eu ante nec ipsum porttitor ultricies sed non velit. Suspendisse potenti. Ut placerat, enim ac luctus gravida, quam urna pulvinar urna, in placerat libero est viverra tortor. Vestibulum ante ipsum primis in faucibus orci luctus et ultrices posuere cubilia curae; Mauris mollis sapien nisi, eu blandit dui commodo quis. Donec eget lobortis urna. Pellentesque mollis sapien sed eros suscipit, nec elementum nulla maximus.</p>',
          mandatoryGroups: [],
          files: [
            {
              key: '176635D0264E5B165AD3AC031A10760F',
              group: '0635F899B3111EED8E9B080756ABDEC9',
              label: 'Žádost o nesnižování zálohových plateb',
              note: '',
              prefix: 'Souhlasím s',
              mime: 'application/pdf',
              mandatory: false,
              idx: 0,
            },
          ],
        },
        note: null,
      },
    },
  ],
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

    const containers = await waitFor(() => screen.getAllByTestId('boxDocumentsToBeAccepted'))

    const checkboxes = containers[0].querySelectorAll('input[type="checkbox"]')
    expect(checkboxes.length).toBe(basicOfferResponse.data[1].body.docs?.files.length)
  })

  it('renders "Sign" button for each document recevied for signature', async () => {
    fetch.mockResponseOnce(JSON.stringify(basicOfferResponse))

    render(<Offer {...mockProps} />)

    const buttons = await waitFor(() => screen.getAllByText(mockProps.labels.signatureBtn))
    expect(buttons.length).toBe(
      basicOfferResponse.data
        .filter(item => item.type === 'docsSign')
        .map(item => item.body.docs?.files)
        .flat().length,
    )
  })

  it('opens sign modal', async () => {
    fetch.mockResponseOnce(JSON.stringify(basicOfferResponse))

    render(<Offer {...mockProps} />)

    const signBtn = await waitFor(() => screen.findByText(mockProps.labels.signatureBtn))
    screen.debug(undefined, 1000000)
    userEvent.click(signBtn)

    const modal = await waitFor(() => screen.getByRole('dialog'))
    expect(modal).toBeInTheDocument()
  })

  it('allows to accept the offer', async () => {
    const offerResponse = {
      data: [
        {
          type: 'docsCheck',
          position: 5,
          header: {
            title: 'Doplňkové služby',
          },
          body: {
            head: null,
            text: null,
            docs: {
              title: 'Dokument(y) k elektronické akceptaci služby',
              params: null,
              text:
                '<p>Nunc eu ante nec ipsum porttitor ultricies sed non velit. Suspendisse potenti. Ut placerat, enim ac luctus gravida, quam urna pulvinar urna, in placerat libero est viverra tortor. Vestibulum ante ipsum primis in faucibus orci luctus et ultrices posuere cubilia curae; Mauris mollis sapien nisi, eu blandit dui commodo quis. Donec eget lobortis urna. Pellentesque mollis sapien sed eros suscipit, nec elementum nulla maximus.</p>',
              mandatoryGroups: [],
              files: [
                {
                  key: '176635D0264E5B165AD3AC031A10760F',
                  group: '0635F899B3111EED8E9B080756ABDEC9',
                  label: 'Žádost o nesnižování zálohových plateb',
                  note: '',
                  prefix: 'Souhlasím s',
                  mime: 'application/pdf',
                  mandatory: false,
                  idx: 0,
                },
              ],
            },
            note: null,
          },
        },
        {
          type: 'confirm',
          position: 10,
          header: {
            title: 'Potvrzení nabídky',
          },
          body: {
            head: null,
            text: 'Stisknutím tlačítka Akceptuji potvrdíte souhlas se zahájením dodávky u innogy.',
            params: [
              {
                title: 'Nesnižování záloh',
                group: '0635F899B3111EED8E9B080756ABDEC9',
              },
            ],
          },
        },
      ],
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

  it('renders custom error message from API', async () => {
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
