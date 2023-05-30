import { Box } from '@components'
import { OfferStoreContext } from '@context'
import { useLabels } from '@hooks'
import { OfferStore } from '@stores'
import { CancelDialog, Suppliers } from '@types'
import classNames from 'classnames'
import { observer } from 'mobx-react-lite'
import React, { FC, useContext } from 'react'
import { Button, Form } from 'react-bootstrap'

interface ConfirmProps {
  t: ReturnType<typeof useLabels>
  setConfirmationModal: (value: boolean) => void
  suppliers?: Suppliers
  cancelDialog?: CancelDialog
}

const Confirm: FC<ConfirmProps> = observer(
  ({ t, suppliers, setConfirmationModal, cancelDialog }) => {
    const store = useContext(OfferStoreContext)

    if (!(store instanceof OfferStore)) {
      return null
    }

    return (
      <div
        className={classNames({
          'd-none': (!store.isLoading && store.error) || !store.offerFetched,
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
                value={store.supplier}
                onChange={event => store.selectSupplier(event.target.value)}
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
              disabled={!store.isOfferReadyToAccept}
            >
              {t('submitBtn')}
            </Button>
            {/* If a user wants to start again, the following button will be visible */}
            {cancelDialog && (
              <Button
                className="ml-3"
                variant="outline-primary"
                type="submit"
                onClick={() => store.setIsUnfinishedOfferModalOpen(true)}
              >
                {t('startOver')}
              </Button>
            )}
          </div>
        </Box>
      </div>
    )
  },
)

export default Confirm
