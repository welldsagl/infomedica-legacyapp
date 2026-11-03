import { forwardRef } from 'react'
import { Anchor } from '@mantine/core'
import { createLink } from '@tanstack/react-router'
import type { AnchorHTMLAttributes } from 'react'
import type { AnchorProps } from '@mantine/core'
import type { LinkComponent } from '@tanstack/react-router'

// Integrazione Mantine + TanStack Router: un <Anchor> Mantine con navigazione tipizzata.
type MantineAnchorProps = Omit<AnchorProps, 'href'> & AnchorHTMLAttributes<HTMLAnchorElement>

const MantineAnchor = forwardRef<HTMLAnchorElement, MantineAnchorProps>((props, ref) => (
  <Anchor ref={ref} {...props} />
))
MantineAnchor.displayName = 'MantineAnchor'

const CreatedAnchorLink = createLink(MantineAnchor)

const AnchorLink: LinkComponent<typeof MantineAnchor> = (props) => (
  <CreatedAnchorLink preload="intent" {...props} />
)

export default AnchorLink
