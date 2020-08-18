import { useCallback, useState, ChangeEvent, FormEvent } from 'react'

// eslint-disable-next-line @typescript-eslint/explicit-module-boundary-types
export const useForm = <T>(initialValues: T, onSubmitCallback?: (values: T) => void) => {
  const [values, setValues] = useState<T>(initialValues)

  const handleInputChange = useCallback(
    (event: ChangeEvent<HTMLInputElement | HTMLTextAreaElement>, id?: string) => {
      setValues({
        ...values,
        [id ? id : event.target.id]: event.target.value,
      })
    },
    [values],
  )

  const handleFormSubmit = (event: FormEvent) => {
    event.preventDefault()
    onSubmitCallback && onSubmitCallback(values)
  }

  return {
    values,
    handleInputChange,
    handleFormSubmit,
    setValues,
  }
}
