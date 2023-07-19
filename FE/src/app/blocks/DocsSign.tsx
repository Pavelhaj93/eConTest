import { Box, BoxHeader, BoxHeading, DocumentLink, Icon, SignButton } from '@components'
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
  bodyNote: NewOfferResponse.Body['note']
  getFileUrl: string
  guid: string
  handleDownload: () => void
  onOpenSignatureModal: (key: string) => void
}

const DocsSign: FC<DocsSignProps> = observer(
  ({
    t,
    headerTitle,
    docsTitle,
    docsText,
    docsFiles,
    bodyNote,
    getFileUrl,
    guid,
    handleDownload,
    onOpenSignatureModal,
  }) => {
    const store = useContext(OfferStoreContext)

    if (!(store instanceof OfferStore)) {
      return null
    }

    return (
      <>
        {headerTitle && (
          <BoxHeader>
            <h2 className="text-center text-white">{headerTitle}</h2>
          </BoxHeader>
        )}
        <Box>
          <h3 className="text-center">{docsTitle}</h3>
          <div
            className="editorial-content text-center my-4"
            dangerouslySetInnerHTML={{
              __html: bodyNote ?? '',
            }}
          />
          {docsFiles.map(({ key, prefix, label, accepted }) => (
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
                    onClick={() => onOpenSignatureModal(key)}
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
                        onClick={() => onOpenSignatureModal(key)}
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
              {bodyNote && <InfoElement value={bodyNote} className="mb-4" />}
            </>
          ))}
        </Box>
      </>
    )
  },
)

export default DocsSign
