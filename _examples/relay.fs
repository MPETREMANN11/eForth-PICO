\ *********************************************************************
\ Red Relay on GPIO 07
\    Filename:      relay.fs
\    Date:          26 dec 2023
\    Updated:       26 dec 2023
\    File Version:  1.0
\    Forth:         eFORTH RP pico
\    Author:        Marc PETREMANN
\    GNU General Public License
\ *********************************************************************


$D0000000 constant SIO_BASE
SIO_BASE $020 + constant GPIO_OE
SIO_BASE $024 + constant GPIO_OE_SET
SIO_BASE $028 + constant GPIO_OE_CLR

SIO_BASE $010 + constant GPIO_OUT
SIO_BASE $014 + constant GPIO_OUT_SET 
SIO_BASE $018 + constant GPIO_OUT_CLR 
SIO_BASE $01c + constant GPIO_OUT_XOR

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

$40014000 constant IO_BANK0_BASE

: GPIO_CTRL ( n -- addr ) 
    8 * 4 + IO_BANK0_BASE + 
  ;

\  1 constant GPIO_FUNC_SPI
\  2 constant GPIO_FUNC_UART
\  3 constant GPIO_FUNC_I2C 
\  4 constant GPIO_FUNC_PWM
 5 constant GPIO_FUNC_SIO
\  6 constant GPIO_FUNC_PIO0
\  7 constant GPIO_FUNC_PIO1
\  8 constant GPIO_FUNC_GPCK
\  9 constant GPIO_FUNC_USB
\ $f constant GPIO_FUNC_NULL

: gpio_set_function ( gpio function -- )
    swap GPIO_CTRL !
  ;

07 constant MY_RELAY   \ Reed Relay on GPIO 7

: relay_init ( -- )
    MY_RELAY GPIO_FUNC_SIO gpio_set_function
    MY_RELAY GPIO_OUT gpio_set_dir
  ;

: relay_state ( state -- )
    MY_RELAY swap gpio_put
  ;

: relay_on ( -- )
    GPIO_HIGH relay_state
  ;

: relay_off ( -- )
    GPIO_LOW  relay_state
  ;


/ *** case of bistable relay ***************************************************

07 constant MY_RELAY_ON   \ Reed Relay on GPIO 7
08 constant MY_RELAY_OFF  \ Reed Relay on GPIO 8

: relays_init ( -- )
    MY_RELAY_ON   GPIO_FUNC_SIO gpio_set_function
    MY_RELAY_ON   GPIO_OUT gpio_set_dir
    MY_RELAY_OFF  GPIO_FUNC_SIO gpio_set_function
    MY_RELAY_OFF  GPIO_OUT gpio_set_dir
  ;

500 value ACTION_DELAY

: bistable_on ( -- )
    MY_RELAY_ON  GPIO_HIGH gpio_put
    ACTION_DELAY ms
    MY_RELAY_ON  GPIO_LOW  gpio_put
  ;

: bistable_off ( -- )
    MY_RELAY_OFF GPIO_HIGH gpio_put
    ACTION_DELAY ms
    MY_RELAY_OFF GPIO_LOW  gpio_put
  ;



