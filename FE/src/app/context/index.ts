import React from 'react'
import { OfferStore, CallMeBackStore } from '@stores'

export const OfferStoreContext = React.createContext<OfferStore | null>(null)
export const CallMeBackStoreContext = React.createContext<CallMeBackStore | null>(null)
