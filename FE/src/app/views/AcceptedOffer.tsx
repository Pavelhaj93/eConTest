import React, { Fragment, useEffect, useState } from 'react'
import classNames from 'classnames'
import { OfferType, View } from '@types'
import { observer } from 'mobx-react-lite'
import { OfferStore } from '@stores'
import { useLabels } from '@hooks'
import { Alert } from 'react-bootstrap'
import { Box, BoxHeading, Icon } from '@components'
import { colors } from '@theme'
import { parseUrl } from '@utils'

export const AcceptedOffer: React.FC<View> = observer(
  ({ guid, offerUrl, labels, getFileUrl, timeout }) => {
    const [store] = useState(() => new OfferStore(OfferType.ACCEPTED, offerUrl, guid))
    const t = useLabels(labels)

    useEffect(() => {
      store.fetchOffer(timeout)
      // eslint-disable-next-line react-hooks/exhaustive-deps
    }, [])

    return (
      <Fragment>
        {store.error && (
          <Alert variant="danger">
            <h3>{t('appUnavailableTitle')}</h3>
            <div
              className="editorial-content"
              dangerouslySetInnerHTML={{ __html: t('appUnavailableText') }}
            />
          </Alert>
        )}

        <Box
          className={classNames({
            loading: store.isLoading || !store.documentGroups.length,
            'd-none': store.error, // hide the whole box if there is an error
          })}
        >
          {store.documentGroups.length > 0 && (
            <Fragment>
              {store.documentGroups.map(({ title, files }, idx) => (
                <Fragment key={idx}>
                  <BoxHeading id={`heading${idx}`}>{title}</BoxHeading>
                  <ul className="list-unstyled" aria-labelledby={`heading${idx}`}>
                    {files.map(({ label, key }) => (
                      <li key={key} className="form-item-wrapper mb-3">
                        <a
                          href={parseUrl(`${getFileUrl}/${key}`, { guid })}
                          className="like-custom-control-label"
                        >
                          <span>{label}</span>
                          <Icon name="pdf" size={36} color={colors.orange} className="ml-auto" />
                        </a>
                      </li>
                    ))}
                  </ul>
                </Fragment>
              ))}
            </Fragment>
          )}
        </Box>
      </Fragment>
    )
  },
)
