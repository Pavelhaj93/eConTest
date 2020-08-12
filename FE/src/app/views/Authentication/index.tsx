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
  placeholder?: string
  helpText?: string
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

  const { values, handleInputChange } = useForm<{
    zip: string
    customerNumber: string
  }>(initialValues)

  const verificationMethods: VerificationMethod[] = useMemo(() => {
    return [
      {
        id: 'zip',
        label: labels.zip as string,
        placeholder: labels.zipPlaceholder,
        helpText: labels.zipHelpText,
      },
      {
        id: 'customerNumber',
        label: labels.customerNumber as string,
        placeholder: labels.customerNumberPlaceholder,
        helpText: labels.customerNumberHelpText,
      },
    ]
  }, [labels])

  // create refs for each method's input
  const methodInputRefs: {
    [key: string]: RefObject<HTMLInputElement>
  } = verificationMethods.reduce((acc, method) => {
    const methodId = `${method.id}Method`
    return {
      ...acc,
      [methodId]: createRef<HTMLInputElement>(),
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

  useEffect(() => {}, [values])

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
        {verificationMethods.map(({ id, label, placeholder, helpText }, idx) => {
          const methodId = `${id}Method`

          return (
            <div className="d-md-flex align-items-md-center" key={`${id}-${idx}`}>
              <Form.Check
                type="radio"
                label={label}
                name="verificationMethod"
                id={methodId}
                onChange={handleChangeMethod}
                custom
              />
              <Media query={{ maxWidth: breakpoints.mdMax }}>
                {matches => {
                  // use different type of component/animation on mobile and desktop
                  const ShowHideComponent = matches ? Collapse : Fade

                  return (
                    <ShowHideComponent
                      in={selectedMethod === methodId}
                      className="mb-2 mb-md-0"
                      onEntered={() => methodInputRefs[methodId].current?.focus()}
                    >
                      <Form.Group controlId={id} className="mb-0 ml-4 ml-md-3">
                        <FormControlTooltipWrapper>
                          <Form.Label srOnly>{label}</Form.Label>
                          <FormControl
                            inputMode="numeric"
                            pattern="[0-9]*"
                            name={id}
                            {...(placeholder ? { placeholder } : {})}
                            ref={methodInputRefs[methodId]}
                            onChange={handleInputChange}
                            className={classNames({
                              invalid:
                                selectedMethod === methodId &&
                                !values[id as keyof typeof initialValues] &&
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
