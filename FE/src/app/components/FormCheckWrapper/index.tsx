import React, { ChangeEvent } from 'react'
import { Form } from 'react-bootstrap'

type Props = {
  type: 'checkbox' | 'radio'
  name: string
  id: string
  value?: string
  checked: boolean
  onChange: (event: ChangeEvent<HTMLInputElement>) => void
}

export const FormCheckWrapper: React.FC<Props> = ({ children, id, ...rest }) => (
  <Form.Check id={id} className="form-check-wrapper mb-3" custom>
    <Form.Check.Input {...rest} />
    {children}
  </Form.Check>
)
