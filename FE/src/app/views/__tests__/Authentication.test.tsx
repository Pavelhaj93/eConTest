import React from 'react'
import { render, screen } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import { Authentication } from '../'

const mockProps = {
  guid: '0635F899B3111EDC9BC4A505A628FCED',
  view: 'Authentication',
  nextUrl: '',
  backUrl: '',
  offerUrl: '/api/documents',
  uploadUrl: 'api/upload',
  errorPageUrl: '/error',
  sessionExpiredPageUrl: '',
  formAction: '/authentication.html',
  choices: [
    {
      label: 'PSČ trvalého bydliště',
      key: 'permanentresidencepostalcode',
      placeholder: '15500',
      helpText: 'PSČ trvalého bydliště hint',
      regex: '^[0-9]{3}([\\s]{0,1}[0-9]{2})$',
    },
    {
      label: 'Zákaznické číslo',
      key: 'identitycardnumber',
      placeholder: '123456789',
      helpText: 'Zákaznické číslo hint',
      regex: '^\\d{10}$',
    },
  ],
  labels: {
    birthDate: '1. Datum narození',
    submitBtn: 'Odeslat',
  },
}

const mockPropsSingleChoice = {
  guid: '0635F899B3111EDC9BC4A505A628FCED',
  view: 'Authentication',
  nextUrl: '',
  backUrl: '',
  offerUrl: '/api/documents',
  uploadUrl: '/api/upload',
  errorPageUrl: '/error',
  sessionExpiredPageUrl: '',
  formAction: '/authentication.html',
  choices: [
    {
      label: 'PSČ trvalého bydliště',
      key: 'permanentresidencepostalcode',
      placeholder: '15500',
      helpText: 'PSČ trvalého bydliště hint',
      regex: '^[0-9]{3}([\\s]{0,1}[0-9]{2})$',
    },
  ],
  labels: {
    birthDate: '1. Datum narození',
    submitBtn: 'Odeslat',
  },
}

describe('Authentication view', () => {
  it('renders submit button', () => {
    render(<Authentication {...mockProps} />)
    const submitBtn = screen.queryByText(mockProps.labels.submitBtn)

    expect(submitBtn).toBeInTheDocument()
    expect(submitBtn).toHaveAttribute('type', 'submit')
  })

  it('renders birh date input', () => {
    render(<Authentication {...mockProps} />)
    const input = screen.queryByLabelText(mockProps.labels.birthDate)

    expect(input).toBeInTheDocument()
  })

  it('should not render radio button for single auth choice', () => {
    render(<Authentication {...mockPropsSingleChoice} />)
    const radioButton = screen.queryByLabelText(mockPropsSingleChoice.choices[0].label, {
      selector: 'input[type="radio"]',
    })

    expect(radioButton).toBe(null)
  })

  it('renders validation message after submitting empty form', () => {
    render(<Authentication {...mockProps} />)
    const submitBtn = screen.getByText(mockProps.labels.submitBtn)

    userEvent.click(submitBtn)

    const alert = screen.getByRole('alert')

    expect(alert).toBeInTheDocument()
  })

  it('renders validation error message when provided', () => {
    const mockPropsWithError = {
      ...mockProps,
      labels: {
        ...mockProps,
        validationError: 'Validation error message',
      },
    }
    render(<Authentication {...mockPropsWithError} />)

    const alert = screen.getByRole('alert')

    expect(alert).toBeInTheDocument()
  })

  it('submit button should not have inactive class after filling and submitting the form', () => {
    render(<Authentication {...mockPropsSingleChoice} />)
    const birthDateInput = screen.getByLabelText(mockPropsSingleChoice.labels.birthDate)
    const zipCodeInput = screen.getByLabelText(mockPropsSingleChoice.choices[0].label)

    // because of datepicker library, I used `paste` instead of `type` function
    userEvent.paste(birthDateInput, '01.11.2020')
    userEvent.type(zipCodeInput, '17000')

    const submitBtn = screen.getByText(mockProps.labels.submitBtn)

    expect(submitBtn).not.toHaveClass('btn-inactive')
  })
})
