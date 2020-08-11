import React, { useState } from 'react'
import { Row, Col, Form, Button } from 'react-bootstrap'
import { subDays } from 'date-fns'
import classNames from 'classnames'
import { View } from '@types'
import { Datepicker } from '@components'

export const Authentication: React.FC<View> = ({ labels, formAction }) => {
  const [isFormValid, setFormValid] = useState(false)
  const [date, setDate] = useState<Date | null>(null)

  return (
    <Form className="mt-4" action={formAction} method="post">
      <Row>
        <Col xs={12} md={8} lg={6}>
          <Form.Group>
            <Form.Label htmlFor="birthDate">{labels.birthDate}</Form.Label>
            <Datepicker
              id="birthDate"
              placeholderText={labels.birthDatePlaceholder}
              maxDate={subDays(new Date(), 1)}
              onChange={(date: Date) => setDate(date)}
              selected={date}
              showYearDropdown
              dropdownMode="select"
              ariaLabelOpen={labels.ariaOpenCalendar}
            />
          </Form.Group>
          <Form.Group>
            <Form.Label>{labels.verificationMethod}</Form.Label>
            <Form.Check
              type="radio"
              label={labels.zip}
              name="verificationMethod"
              id="verificationMethod1"
              custom
            />
            <Form.Check
              type="radio"
              label={labels.customerNumber}
              name="verificationMethod"
              id="verificationMethod2"
              custom
            />
          </Form.Group>
        </Col>
      </Row>
      <Button
        variant="primary"
        type="submit"
        className={classNames({ 'btn-block-mobile': true, 'btn-inactive': !isFormValid })}
      >
        {labels.submitBtn}
      </Button>
    </Form>
  )
}
