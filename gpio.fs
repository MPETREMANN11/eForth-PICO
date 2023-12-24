\ *********************************************************************
\ GPIO general library
\    Filename:      gpio.fs
\    Date:          24 dec 2023
\    Updated:       24 dec 2023
\    File Version:  1.0
\    Forth:         eFORTH RP pico
\    Author:        Marc PETREMANN
\    GNU General Public License
\ *********************************************************************

\ Essentials words:

\ gpio_set_function ( gpio function -- )
\  Parameters:
\  - gpio: between [2..30]
\  - function: 
\     GPIO_FUNC_SPI
\     GPIO_FUNC_UART
\     GPIO_FUNC_I2C 
\     GPIO_FUNC_PWM
\     GPIO_FUNC_SIO
\     GPIO_FUNC_PIO0
\     GPIO_FUNC_PIO1
\     GPIO_FUNC_GPCK
\     GPIO_FUNC_USB
\     GPIO_FUNC_NULL

\ gpio_set_dir    ( gpio direction -- ) 
\  Parameters:
\  - gpio: between [2..30]
\  - direction: GPIO_OUT | GPIO_IN 

\ gpio_put ( gpio state -- )
\  Parameters:
\  - gpio: between [2..30]
\  - state: GPIO_HIGH | GPIO_LOW




$D0000000 constant SIO_BASE
SIO_BASE $020 + constant GPIO_OE
SIO_BASE $024 + constant GPIO_OE_SET
SIO_BASE $028 + constant GPIO_OE_CLR

SIO_BASE $004 + constant GPIO_IN        \ Input value for GPIO pins [0..30]

SIO_BASE $010 + constant GPIO_OUT       \ GPIO output value
SIO_BASE $014 + constant GPIO_OUT_SET   \ GPIO output value set
SIO_BASE $018 + constant GPIO_OUT_CLR   \ GPIO output value clear
SIO_BASE $01c + constant GPIO_OUT_XOR   \ GPIO output value XOR


\ transform GPIO number in his binary mask
: PIN_MASK ( n -- mask )
    1 swap lshift
  ;

1 constant GPIO_OUT   \ set direction OUTput mode
0 constant GPIO_IN    \ set direction INput mode

\ set direction for selected gpio
: gpio_set_dir  ( gpio direction -- )
    if     PIN_MASK GPIO_OE_SET !
    else   PIN_MASK GPIO_OE_CLR !    then
  ;

1 constant GPIO_HIGH    \ set GPIO state
0 constant GPIO_LOW     \ set GPIO state

\ set GPIO on/off
: gpio_put ( gpio state -- )
    if     PIN_MASK GPIO_OUT_SET !
    else   PIN_MASK GPIO_OUT_CLR !    then
  ;

\ Get state of a single specified GPIO 
: gpio_get ( gpio -- state )
    PIN_MASK GPIO_IN @
  ;

$40014000 constant IO_BANK0_BASE

: GPIO_CTRL ( n -- addr ) 
    8 * 4 + IO_BANK0_BASE + 
  ;

 1 constant GPIO_FUNC_SPI
 2 constant GPIO_FUNC_UART
 3 constant GPIO_FUNC_I2C 
 4 constant GPIO_FUNC_PWM
 5 constant GPIO_FUNC_SIO
 6 constant GPIO_FUNC_PIO0
 7 constant GPIO_FUNC_PIO1
 8 constant GPIO_FUNC_GPCK
 9 constant GPIO_FUNC_USB
$f constant GPIO_FUNC_NULL

: gpio_set_function ( gpio function -- )
    swap GPIO_CTRL !
  ;

$1F constant IO_BANK0_GPIO0_CTRL_FUNCSEL_BITS

: gpio_get_function ( gpio -- function )
    GPIO_CTRL @
    IO_BANK0_GPIO0_CTRL_FUNCSEL_BITS and
  ;

25 gpio_get_function .
 1 gpio_get_function .

