import { Box, BoxHeading, DocumentLink, Icon, SignButton } from '@components'
import { breakpoints, colors } from '@theme'
import React, { FC, Fragment, useContext } from 'react'

import InfoElement from './InfoElement'
import { NewOfferResponse } from '@types'
import { parseUrl } from '@utils'
import { useLabels } from '@hooks'
import Media from 'react-media'
import { OfferStoreContext } from '@context'
import { OfferStore } from '@stores'
import { observer } from 'mobx-react-lite'

interface DocsSignProps {
  t: ReturnType<typeof useLabels>
  headerTitle: NewOfferResponse.Header['title']
  docsTitle: NewOfferResponse.Docs['title']
  docsText: NewOfferResponse.Docs['text']
  docsFiles: NewOfferResponse.Docs['files']
  getFileUrl: string | undefined
  guid: string
  handleDownload: () => void
  openSignatureModal: (key: string) => void
}

const DocsSign: FC<DocsSignProps> = observer(
  ({ t, docsTitle, docsText, docsFiles, getFileUrl, guid, handleDownload, openSignatureModal }) => {
    const store = useContext(OfferStoreContext)

    if (!(store instanceof OfferStore)) {
      return null
    }

    return (
      <Fragment>
        {store.docGroupsToBeSigned.length > 0 && (
          <Box>
            <BoxHeading>{docsTitle}</BoxHeading>
            <div
              className="editorial-content text-center my-4"
              dangerouslySetInnerHTML={{
                __html: docsText ?? '',
              }}
            />
            {docsFiles.map(({ key, prefix, label, accepted, note }) => (
              <>
                <div key={key} className="form-item-wrapper mb-3">
                  <div className="like-custom-control-label">
                    {accepted && (
                      <Icon
                        name="check-circle"
                        size={36}
                        color={colors.green}
                        className="form-item-wrapper__icon mr-2"
                      />
                    )}
                    <span>
                      {prefix}{' '}
                      <DocumentLink
                        url={parseUrl(`${getFileUrl}/${key}?t=${new Date().getTime()}`, {
                          guid,
                        })}
                        label={label}
                        onClick={handleDownload}
                        noIcon
                      />
                    </span>
                    <SignButton
                      className="d-none d-sm-block"
                      signed={accepted ? accepted : false}
                      onClick={() => openSignatureModal(key)}
                      labelSign={t('signatureBtn')}
                      labelEdit={t('signatureEditBtn')}
                      descriptionId={'signBtnDescription'}
                      showLabelEdit={false}
                    />
                  </div>
                  <Media query={{ maxWidth: breakpoints.smMax }}>
                    {(matches: any) =>
                      matches && (
                        <SignButton
                          className="btn-block-mobile mt-3"
                          signed={accepted ? accepted : false}
                          onClick={() => openSignatureModal(key)}
                          labelSign={t('signatureBtn')}
                          labelEdit={t('signatureEditBtn')}
                          descriptionId={'signBtnDescription'}
                          showLabelEdit={true}
                        />
                      )
                    }
                  </Media>
                </div>
                {/* info text */}
                {note && <InfoElement value={note} className="mb-4" />}
              </>
            ))}
            <div
              id="signBtnDescription"
              className="editorial-content text-muted small"
              dangerouslySetInnerHTML={{
                __html: t('signatureNote'),
              }}
              aria-hidden="true"
            />
          </Box>
        )}
      </Fragment>
    )
  },
)

export default DocsSign
