import React from 'react'
import { Form } from 'react-bootstrap'
import { Icon, Name } from '@components'
import { colors } from '@theme'

type Props = {
  type: 'checkbox' | 'radio'
  name: string
  id: string
  label: string
  value: string
  checked?: boolean
  iconName?: Name
}

export const FormCheckWrapper: React.FC<Props> = ({
  type,
  name,
  id,
  label,
  value,
  checked,
  iconName = null,
}) => (
  <Form.Check
    type={type}
    id={id}
    name={name}
    checked={checked}
    value={value}
    className="form-check-wrapper mb-3"
    custom
  >
    <Form.Check.Input type={type} />
    <Form.Check.Label>
      <span>{label}</span>
      {iconName && <Icon name={iconName} size={36} color={colors.orange} className="ml-auto" />}
    </Form.Check.Label>
  </Form.Check>
)
