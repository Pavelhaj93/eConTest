import { Box, BoxHeader, DocumentLink, Icon, SignButton } from '@components'
import { breakpoints, colors } from '@theme'
import React, { FC, Fragment, useContext } from 'react'

import InfoElement from './InfoElement'
import { CommodityProductType, NewOfferResponse } from '@types'
import { getColorByCommodityType, getCommodityTitle, parseUrl } from '@utils'
import { useLabels } from '@hooks'
import Media from 'react-media'
import { OfferStoreContext } from '@context'
import { OfferStore } from '@stores'
import { observer } from 'mobx-react-lite'

interface DocsSignProps {
  t: ReturnType<typeof useLabels>
  type?: CommodityProductType
  headerTitle: NewOfferResponse.Header['title']
  docsTitle: NewOfferResponse.Docs['title']
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
    type,
    headerTitle,
    docsTitle,
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
        {headerTitle && !type && (
          <BoxHeader>
            <h2 className="text-center text-white">{headerTitle}</h2>
          </BoxHeader>
        )}
        {type === CommodityProductType.GAS && (
          <BoxHeader backgroundColor={getColorByCommodityType(CommodityProductType.GAS)}>
            <Icon name={type} width={30} />
            <h2 className="text-center text-white ml-3">
              {getCommodityTitle(CommodityProductType.GAS, t)}
            </h2>
          </BoxHeader>
        )}
        {type === CommodityProductType.ELECTRICITY && (
          <BoxHeader backgroundColor={getColorByCommodityType(CommodityProductType.ELECTRICITY)}>
            <Icon name={type} width={30} />
            <h2 className="text-center text-white ml-3">
              {getCommodityTitle(CommodityProductType.ELECTRICITY, t)}
            </h2>
          </BoxHeader>
        )}
        {type === CommodityProductType.BOTH && (
          <div className="d-flex">
            <BoxHeader
              backgroundColor={getColorByCommodityType(CommodityProductType.ELECTRICITY)}
              className="w-50 mr-2 mb-3"
            >
              <Icon name={CommodityProductType.ELECTRICITY} width={30} />
              <h2 className="text-center text-white ml-3">
                {getCommodityTitle(CommodityProductType.ELECTRICITY, t)}
              </h2>
            </BoxHeader>
            <BoxHeader
              backgroundColor={getColorByCommodityType(CommodityProductType.GAS)}
              className="w-50 ml-2 mb-3"
            >
              <Icon name={CommodityProductType.GAS} width={30} />
              <h2 className="text-center text-white ml-3">
                {getCommodityTitle(CommodityProductType.GAS, t)}
              </h2>
            </BoxHeader>
          </div>
        )}
        <Box className="mb-4">
          {type && headerTitle && <h2 className="text-center">{headerTitle}</h2>}
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
