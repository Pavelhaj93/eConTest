import React, {
  useState,
  useCallback,
  ChangeEvent,
  createRef,
  RefObject,
  FormEvent,
  useEffect,
  useMemo,
  useRef,
  MouseEvent,
} from 'react'
import { Row, Col, Form, Button, FormControl, Collapse, Alert } from 'react-bootstrap'
import { subDays } from 'date-fns'
import classNames from 'classnames'
import { View } from '@types'
import { Box, Datepicker, FormControlTooltipWrapper, Icon, Tooltip } from '@components'
import { useForm } from '@hooks'
import { colors } from '@theme'

type FormValues = {
  [key: string]: string
}

export const Authentication: React.FC<View> = ({
  hideInnogyAccount = false,
  gaEventClickData,
  labels,
  formAction,
  innogyAccountUrl,
  choices = [],
}) => {
  const [isFormValid, setFormValid] = useState(false)
  const [date, setDate] = useState<Date | null>(null)
  const [selectedChoice, setSelectedChoice] = useState<string | null>(null)
  const [wasValidated, setValidated] = useState(false)
  const formRef = useRef<HTMLFormElement>(null)

  const initialValues = useMemo(
    () =>
      choices.reduce((acc, choice) => {
        return {
          ...acc,
          [choice.key]: '',
        }
      }, {}),
    [choices],
  )
  const { values, handleInputChange } = useForm<FormValues>(initialValues)

  // create refs for each choice's input
  const choiceInputRefs: {
    [key: string]: RefObject<HTMLInputElement>
  } = choices.reduce((acc, choice) => {
    return {
      ...acc,
      [choice.key]: createRef<HTMLInputElement>(),
    }
  }, {})

  const handleChangeChoice = useCallback((event: ChangeEvent<HTMLInputElement>) => {
    event.persist()
    setSelectedChoice(event.target.value)
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

  const handleLoginButtonClicked = (e: MouseEvent<HTMLElement>) => {
    if (!gaEventClickData) return true

    e.preventDefault()

    if (!window.dataLayer) {
      window.dataLayer = []
    }

    window.dataLayer.push({
      event: 'gaEvent',
      gaEventData: gaEventClickData,
      eventCallback: function () {
        window.location.href = innogyAccountUrl as string
      },
    })
  }

  /**
   * Validate field value by provided regular expression.
   */
  const isFieldValid = useCallback((value: string, expression: RegExp | string): boolean => {
    // if expression is an empty string => do not validate
    if (!expression) {
      return true
    }

    const re = new RegExp(expression)
    return re.test(value)
  }, [])

  // 0. if we have one choice only, set it as selected when mounted
  // 1. append generated token into login form
  useEffect(() => {
    if (choices.length === 1) {
      setSelectedChoice(choices[0].key)
    }

    const tokenInput = document.querySelector('#token input')
    if (tokenInput) {
      formRef.current?.appendChild(tokenInput)
    }

    // not a nice hack to unselected checked radio when using browser back button
    setTimeout(() => {
      const checkedRadio = formRef.current?.querySelector('input:checked') as HTMLInputElement
      if (checkedRadio) {
        checkedRadio.checked = false
      }
    }, 1)
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [])

  // 2. if choice is changed, mark the form as invalid
  useEffect(() => {
    setFormValid(false)
  }, [selectedChoice])

  // 3. when some choice value is changed, check whether form can be marked as valid
  useEffect(() => {
    const choice = choices.find(choice => choice.key === selectedChoice)

    if (!choice) {
      return
    }

    // value for the selected choice
    const value = values[choice.key]

    // if
    // 1. choice value is valid
    // 2. choice is selected
    // 3. date is filled
    // then form is good to go
    if (isFieldValid(value, choice.regex ?? '') && selectedChoice && date) {
      setFormValid(true)
    } else {
      setFormValid(false)
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [values, date, selectedChoice])

  return (
    <Row className="justify-content-center">
      <Col xs={12} lg={6} className="d-flex flex-column">
        <Box backgroundColor="gray-10" className="flex-grow-1 px-sm-4">
          <Row className="justify-content-center">
            <Col xs={12} md={10} lg={12}>
              <Form
                noValidate
                action={formAction}
                method="post"
                onSubmit={handleSubmit}
                ref={formRef}
              >
                {(wasValidated || labels.validationError) && (
                  <Alert variant="danger" className="mt-0">
                    <div
                      dangerouslySetInnerHTML={{
                        __html: wasValidated ? labels.requiredFields : labels.validationError,
                      }}
                    />
                  </Alert>
                )}

                <Row>
                  <Col xs={12}>
                    <Form.Group>
                      <Form.Label
                        htmlFor="birthDate"
                        className={classNames({ 'text-danger': !date && wasValidated })}
                      >
                        {labels.birthDate}
                      </Form.Label>
                      <FormControlTooltipWrapper>
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
                          nextMonthButtonLabel={labels.ariaNextMonth}
                          previousMonthButtonLabel={labels.ariaPreviousMonth}
                          chooseDayAriaLabelPrefix={labels.ariaChooseDay}
                        />
                        {labels.birthDateHelpText && <Tooltip>{labels.birthDateHelpText}</Tooltip>}
                      </FormControlTooltipWrapper>
                    </Form.Group>
                  </Col>
                </Row>

                <Form.Group>
                  {choices.length > 1 && (
                    <div
                      className={classNames({
                        'like-label': true,
                        'text-danger': !selectedChoice && wasValidated,
                      })}
                    >
                      {labels.verificationMethod}
                    </div>
                  )}
                  {/* render multiple choices */}
                  {choices.length > 1 ? (
                    choices.map(({ key, label, placeholder, helpText, regex }, idx) => {
                      const isVisible = selectedChoice === key

                      return (
                        <div key={`${key}-${idx}`}>
                          <Form.Check
                            type="radio"
                            label={label}
                            name="key"
                            id={`key-${idx}`}
                            onChange={handleChangeChoice}
                            value={key}
                            custom
                          />
                          <Collapse
                            in={isVisible}
                            className="mb-2 mb-md-0"
                            onEntered={() => choiceInputRefs[key].current?.focus()}
                          >
                            <Form.Group controlId={key} className="mb-0 ml-4 ml-md-3">
                              <FormControlTooltipWrapper>
                                <Form.Label srOnly>{label}</Form.Label>
                                <FormControl
                                  type="text"
                                  inputMode="numeric"
                                  pattern="[0-9]*"
                                  name="value"
                                  {...(placeholder ? { placeholder } : {})}
                                  ref={choiceInputRefs[key]}
                                  // use custom "id" instead of the one on input element
                                  onChange={event =>
                                    handleInputChange(
                                      event as ChangeEvent<HTMLInputElement | HTMLTextAreaElement>,
                                      key,
                                    )
                                  }
                                  className={classNames({
                                    invalid:
                                      isVisible &&
                                      !isFieldValid(values[key], regex ?? '') &&
                                      wasValidated,
                                  })}
                                  tabIndex={isVisible ? 0 : -1}
                                  // do not send multiple fields
                                  disabled={key !== selectedChoice}
                                />
                                {helpText && <Tooltip visible={isVisible}>{helpText}</Tooltip>}
                              </FormControlTooltipWrapper>
                            </Form.Group>
                          </Collapse>
                        </div>
                      )
                    })
                  ) : (
                    // render single choice
                    <Row>
                      <Col xs={12}>
                        <input type="hidden" name="key" value={choices[0].key} />
                        <Form.Label htmlFor={choices[0].key}>{choices[0].label}</Form.Label>
                        <FormControlTooltipWrapper>
                          <FormControl
                            type="text"
                            inputMode="numeric"
                            pattern="[0-9]*"
                            id={choices[0].key}
                            name="value"
                            {...(choices[0].placeholder
                              ? { placeholder: choices[0].placeholder }
                              : {})}
                            // use custom "id" instead of the one on input element
                            onChange={event =>
                              handleInputChange(
                                event as ChangeEvent<HTMLInputElement | HTMLTextAreaElement>,
                                choices[0].key,
                              )
                            }
                            className={classNames({
                              invalid:
                                !isFieldValid(values[choices[0].key], choices[0].regex ?? '') &&
                                wasValidated,
                            })}
                          />
                          {choices[0].helpText && <Tooltip>{choices[0].helpText}</Tooltip>}
                        </FormControlTooltipWrapper>
                      </Col>
                    </Row>
                  )}
                </Form.Group>

                <Button
                  variant="secondary"
                  type="submit"
                  className={classNames({ 'btn-block-mobile': true, 'btn-inactive': !isFormValid })}
                >
                  {labels.submitBtn}
                </Button>
              </Form>
            </Col>
          </Row>
        </Box>
      </Col>
      {!hideInnogyAccount && (
        <Col xs={12} lg={6} className="d-flex flex-column">
          <Box backgroundColor="gray-80" className="d-flex flex-column flex-grow-1 px-4">
            <Row className="justify-content-center d-flex h-100">
              <Col xs={12} md={10} lg={12} className="d-flex flex-column">
                {labels.innogyAccountHeading && (
                  <h3 className="mb-4">{labels.innogyAccountHeading}</h3>
                )}
                {labels.innogyAccountBenefits && (
                  <ul className="list-icon list-icon--check flex-grow-1">
                    {(labels.innogyAccountBenefits as string[]).map((text, idx) => (
                      <li key={idx}>{text}</li>
                    ))}
                  </ul>
                )}
                <Button
                  variant="primary"
                  href={innogyAccountUrl}
                  className="btn-block-mobile align-self-start justify-content-center v-center"
                  onClick={handleLoginButtonClicked}
                >
                  <Icon name="key" size={28} color={colors.white} />
                  <span>{labels.innogyAccountBtn}</span>
                </Button>
              </Col>
            </Row>
          </Box>
        </Col>
      )}
    </Row>
  )
}
