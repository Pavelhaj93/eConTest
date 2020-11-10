import React, { Fragment, useEffect, useState } from 'react'
import classNames from 'classnames'
import { View } from '@types'
import { observer } from 'mobx-react-lite'
import { OfferStore } from '@stores'
import { useLabels } from '@hooks'
import { Alert } from 'react-bootstrap'
import { Box, BoxHeading, Icon } from '@components'
import { colors } from '@theme'

export const AcceptedOffer: React.FC<View> = observer(
  ({ doxReadyUrl, labels, getFileUrl, doxTimeout }) => {
    const [store] = useState(() => new OfferStore(doxReadyUrl))
    const t = useLabels(labels)

    useEffect(() => {
      store.fetchDocuments(doxTimeout)
      // eslint-disable-next-line react-hooks/exhaustive-deps
    }, [])

    return (
      <Fragment>
        {store.error && (
          <Alert variant="danger">
            <h3>{t('appUnavailableTitle')}</h3>
            <div>{t('appUnavailableText')}</div>
          </Alert>
        )}

        <Box
          className={classNames({
            loading: store.isLoading,
            'd-none': !store.isLoading && store.error, // hide the whole box if there is an error
          })}
        >
          {store.documents.length > 0 && (
            <Fragment>
              <BoxHeading>Dodatek a přidružené dokumenty</BoxHeading>
              {store.documents.map(({ id, title }) => (
                <div key={id} className="form-item-wrapper mb-3">
                  <a href={`${getFileUrl}${id}`} className="like-custom-control-label">
                    <span>{title}</span>
                    <Icon name="pdf" size={36} color={colors.orange} className="ml-auto" />
                  </a>
                </div>
              ))}
            </Fragment>
          )}
        </Box>
      </Fragment>
    )
  },
)
