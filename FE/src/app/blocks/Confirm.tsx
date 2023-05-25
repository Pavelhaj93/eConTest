import { Box } from '@components'
import { useLabels } from '@hooks'
import { CancelDialog, Supplier, Suppliers } from '@types'
import classNames from 'classnames'
import React, { FC } from 'react'
import { Button, Form } from 'react-bootstrap'

interface ConfirmProps {
  t: ReturnType<typeof useLabels>
  isLoading: boolean
  error: boolean
  offerFetched: boolean
  suppliers: Suppliers | undefined
  supplier: string
  selectSupplier: (supplier: string) => void
  setConfirmationModal: (value: boolean) => void
  isOfferReadyToAccept: boolean
  setIsUnfinishedOfferModalOpen: (value: boolean) => void
  cancelDialog: CancelDialog | undefined
}

const Confirm: FC<ConfirmProps> = ({
  t,
  isLoading,
  error,
  offerFetched,
  suppliers,
  supplier,
  selectSupplier,
  setConfirmationModal,
  isOfferReadyToAccept,
  setIsUnfinishedOfferModalOpen,
  cancelDialog,
}) => {
  return (
    <div
      className={classNames({
        'd-none': (!isLoading && error) || !offerFetched,
      })}
    >
      <h2 className="mt-5 text-center">{t('acceptOfferTitle')}</h2>
      <Box>
        {suppliers && (
          <Form.Group>
            <Form.Label htmlFor="supplier">{suppliers.label}</Form.Label>
            <Form.Control
              as="select"
              id="supplier"
              value={supplier}
              onChange={event => selectSupplier(event.target.value)}
            >
              {[{ label: '', value: '' }, ...suppliers.items].map(({ label, value }, idx) => (
                <option key={idx} value={value}>
                  {label}
                </option>
              ))}
            </Form.Control>
          </Form.Group>
        )}
        <div className="text-center">
          <div
            className="editorial-content mb-3"
            dangerouslySetInnerHTML={{ __html: t('acceptOfferHelptext') }}
          />
          <Button
            variant="secondary"
            type="submit"
            onClick={() => setConfirmationModal(true)}
            disabled={!isOfferReadyToAccept}
          >
            {t('submitBtn')}
          </Button>
          {/* If a user wants to start again, the following button will be visible */}
          {cancelDialog && (
            <Button
              className="ml-3"
              variant="outline-primary"
              type="submit"
              onClick={() => setIsUnfinishedOfferModalOpen(true)}
            >
              {t('startOver')}
            </Button>
          )}
        </div>
      </Box>
    </div>
  )
}

export default Confirm
