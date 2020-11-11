import React from 'react'
import { render, screen, fireEvent } from '@testing-library/react'
import { Authentication } from '../'

const mockProps = {
  view: 'Authentication',
  doxReadyUrl: '/api/documents',
  isAgreed: false,
  isRetention: false,
  isAcquisition: true,
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
  view: 'Authentication',
  doxReadyUrl: '/api/documents',
  isAgreed: false,
  isRetention: false,
  isAcquisition: true,
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

  it('should render validation message after submitting empty form', () => {
    render(<Authentication {...mockProps} />)
    const submitBtn = screen.getByText(mockProps.labels.submitBtn)

    fireEvent.click(submitBtn)

    const alert = screen.getByRole('alert')

    expect(alert).toBeInTheDocument()
  })
})
