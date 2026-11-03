import { forwardRef } from 'react'
import { Button } from '@mantine/core'
import { createLink } from '@tanstack/react-router'
import type { AnchorHTMLAttributes } from 'react'
import type { ButtonProps } from '@mantine/core'
import type { LinkComponent } from '@tanstack/react-router'

// Un <Button> Mantine che naviga come un link (tasto destro, apri in nuova scheda, ...).
type MantineButtonAnchorProps = Omit<ButtonProps, 'href'> & AnchorHTMLAttributes<HTMLAnchorElement>

const MantineButtonAnchor = forwardRef<HTMLAnchorElement, MantineButtonAnchorProps>(
  (props, ref) => <Button component="a" ref={ref} {...props} />
)
MantineButtonAnchor.displayName = 'MantineButtonAnchor'

const CreatedButtonLink = createLink(MantineButtonAnchor)

const ButtonLink: LinkComponent<typeof MantineButtonAnchor> = (props) => (
  <CreatedButtonLink preload="intent" {...props} />
)

export default ButtonLink
