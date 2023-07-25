import { Box, BoxHeader } from '@components'
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
  headerTitle: string
  bodyText: string
  setConfirmationModal: (value: boolean) => void
  suppliers?: Suppliers
  cancelDialog?: CancelDialog
}

const Confirm: FC<ConfirmProps> = observer(
  ({ t, suppliers, setConfirmationModal, headerTitle, bodyText, cancelDialog }) => {
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
        {headerTitle && (
          <BoxHeader>
            <h2 className="text-center text-white">{t('acceptOfferTitle')}</h2>
          </BoxHeader>
        )}
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
              dangerouslySetInnerHTML={{ __html: bodyText }}
            />
            {!suppliers && <div className="mb-5"></div>}
            <Button
              variant="primary"
              type="submit"
              onClick={() => setConfirmationModal(true)}
              disabled={!store.isOfferReadyToAccept}
            >
              {t('submitBtn')}
            </Button>
          </div>
        </Box>
      </div>
    )
  },
)

export default Confirm
