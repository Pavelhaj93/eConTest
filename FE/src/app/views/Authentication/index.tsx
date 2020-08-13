import React, {
  useState,
  useCallback,
  ChangeEvent,
  useMemo,
  createRef,
  RefObject,
  FormEvent,
  useEffect,
} from 'react'
import { Row, Col, Form, Button, FormControl, Collapse, Fade, Alert } from 'react-bootstrap'
import { subDays } from 'date-fns'
import classNames from 'classnames'
import Media from 'react-media'
import { View } from '@types'
import { Datepicker, FormControlTooltipWrapper, Tooltip } from '@components'
import { breakpoints } from '@theme'
import { useForm } from '@hooks'

type VerificationMethod = {
  label: string
  id: string
  name: string
  placeholder?: string
  helpText?: string
  expression: RegExp
}

type FormValues = {
  zip: string
  customerNumber: string
}

const initialValues = {
  zip: '',
  customerNumber: '',
}

export const Authentication: React.FC<View> = ({ labels, formAction }) => {
  const [isFormValid, setFormValid] = useState(false)
  const [date, setDate] = useState<Date | null>(null)
  const [selectedMethod, setSelectedMethod] = useState<string | null>(null)
  const [wasValidated, setValidated] = useState(false)

  const { values, handleInputChange } = useForm<FormValues>(initialValues)

  const verificationMethods: VerificationMethod[] = useMemo(() => {
    return [
      {
        id: 'zip',
        name: 'zipMethod',
        label: labels.zip as string,
        placeholder: labels.zipPlaceholder,
        helpText: labels.zipHelpText,
        expression: /^\d{5}$/,
      },
      {
        id: 'customerNumber',
        name: 'customerNumberMethod',
        label: labels.customerNumber as string,
        placeholder: labels.customerNumberPlaceholder,
        helpText: labels.customerNumberHelpText,
        expression: /^\d{10}$/,
      },
    ]
  }, [labels])

  // create refs for each method's input
  const methodInputRefs: {
    [key: string]: RefObject<HTMLInputElement>
  } = verificationMethods.reduce((acc, method) => {
    return {
      ...acc,
      [method.name]: createRef<HTMLInputElement>(),
    }
  }, {})

  const handleChangeMethod = useCallback((event: ChangeEvent<HTMLInputElement>) => {
    event.persist()
    setSelectedMethod(event.target.id)
  }, [])

  const handleSubmit = useCallback(
    (event: FormEvent) => {
      if (!isFormValid) {
        event.preventDefault()
        event.stopPropagation()
        setValidated(true)
      }
    },
    [isFormValid],
  )

  /**
   * Validate field value by provided regular expression.
   */
  const isFieldValid = useCallback((value: string, expression: RegExp): boolean => {
    const re = new RegExp(expression)
    return re.test(value)
  }, [])

  // 1. if verification method is changed, mark the form as invalid
  useEffect(() => {
    setFormValid(false)
  }, [selectedMethod])

  // 2. when some method value is changed, check whether form can be marked as valid
  useEffect(() => {
    const method = verificationMethods.find(method => method.name === selectedMethod)

    if (!method) {
      return
    }

    // value for the selected method
    const value = values[method.id as keyof FormValues]

    // if
    // 1. method value is valid
    // 2. method is selected
    // 3. date is filled
    // then form is good to go
    if (isFieldValid(value, method.expression) && selectedMethod && date) {
      setFormValid(true)
    } else {
      setFormValid(false)
    }
  }, [values, date, selectedMethod])

  return (
    <Form noValidate className="mt-4" action={formAction} method="post" onSubmit={handleSubmit}>
      {wasValidated && <Alert variant="danger">{labels.requiredFields}</Alert>}

      <Row>
        <Col xs={12} md={8} lg={6}>
          <Form.Group>
            <Form.Label
              htmlFor="birthDate"
              className={classNames({ 'text-danger': !date && wasValidated })}
            >
              {labels.birthDate}
            </Form.Label>
            <Datepicker
              id="birthDate"
              placeholderText={labels.birthDatePlaceholder}
              maxDate={subDays(new Date(), 1)}
              onChange={(date: Date) => setDate(date)}
              selected={date}
              showYearDropdown
              dropdownMode="select"
              ariaLabelOpen={labels.ariaOpenCalendar}
              isInvalid={!date && wasValidated}
            />
          </Form.Group>
        </Col>
      </Row>

      <Form.Group>
        <div
          className={classNames({
            'like-label': true,
            'text-danger': !selectedMethod && wasValidated,
          })}
        >
          {labels.verificationMethod}
        </div>
        {/* render verification methods */}
        {verificationMethods.map(({ id, name, label, placeholder, helpText, expression }, idx) => {
          return (
            <div className="d-md-flex align-items-md-center" key={`${id}-${idx}`}>
              <Form.Check
                type="radio"
                label={label}
                name="verificationMethod"
                id={name}
                onChange={handleChangeMethod}
                value={id}
                custom
              />
              <Media query={{ maxWidth: breakpoints.mdMax }}>
                {matches => {
                  // use different type of component/animation on mobile and desktop
                  const ShowHideComponent = matches ? Collapse : Fade

                  return (
                    <ShowHideComponent
                      in={selectedMethod === name}
                      className="mb-2 mb-md-0"
                      onEntered={() => methodInputRefs[name].current?.focus()}
                    >
                      <Form.Group controlId={id} className="mb-0 ml-4 ml-md-3">
                        <FormControlTooltipWrapper>
                          <Form.Label srOnly>{label}</Form.Label>
                          <FormControl
                            inputMode="numeric"
                            pattern="[0-9]*"
                            name={id}
                            {...(placeholder ? { placeholder } : {})}
                            ref={methodInputRefs[name]}
                            onChange={handleInputChange}
                            className={classNames({
                              invalid:
                                selectedMethod === name &&
                                !isFieldValid(values[id as keyof FormValues], expression) &&
                                wasValidated,
                            })}
                          />
                          {helpText && <Tooltip id="zipHelpText">{helpText}</Tooltip>}
                        </FormControlTooltipWrapper>
                      </Form.Group>
                    </ShowHideComponent>
                  )
                }}
              </Media>
            </div>
          )
        })}
      </Form.Group>

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
