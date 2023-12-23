\ *********************************************************************
\ BLINK examples on LED 25
\    Filename:      blink.fs
\    Date:          23 dec 2023
\    Updated:       23 dec 2023
\    File Version:  1.0
\    Forth:         eFORTH RP pico
\    Author:        Marc PETREMANN
\    GNU General Public License
\ *********************************************************************


\ *** BLINK minimal version ****************************************************

\ $400140CC ( GPIO_CTRL )
\ $D000001C ( GPIO_OUT_XOR )
\ $D0000020 ( GPIO_OE )
\ $D0000024 ( GPIO_OE_SET )
\ $02000000 ( LED 25 mask )
\ $00000400 ( LED 10 mask )

: blink ( -- )
    5 $400140CC !
    $02000000 $D0000024 !
    begin
        $02000000 $D000001C !
        300 ms
    key? until
  ;


\ *** BLINK version with SIO_BASE **********************************************

$D0000000 constant SIO_BASE
SIO_BASE $020 constant GPIO_OE
SIO_BASE $024 constant GPIO_OE_SET
SIO_BASE $028 constant GPIO_OE_CLR

: blink ( -- )
    5 $400140CC !
    $02000000 GPIO_OE_SET !
    begin
        $02000000 $D000001C !
        300 ms
    key? until
  ;


\ *** BLINK version with PIN_MASK **********************************************

$D0000000 constant SIO_BASE
SIO_BASE $020 + constant GPIO_OE
SIO_BASE $024 + constant GPIO_OE_SET
SIO_BASE $028 + constant GPIO_OE_CLR

SIO_BASE $01c + constant GPIO_OUT_XOR

25 constant ONBOARD_LED

\ transform GPIO number in his binary mask
: PIN_MASK ( n -- mask )
    1 swap lshift
  ;

: blink ( -- )
    5 $400140CC !
    ONBOARD_LED PIN_MASK GPIO_OE_SET !
    begin
        ONBOARD_LED PIN_MASK GPIO_OUT_XOR !
        300 ms
    key? until
  ;


\ *** BLINK version with factorisation code ************************************

$D0000000 constant SIO_BASE
SIO_BASE $020 + constant GPIO_OE
SIO_BASE $024 + constant GPIO_OE_SET
SIO_BASE $028 + constant GPIO_OE_CLR

SIO_BASE $010 + constant GPIO_OUT
SIO_BASE $014 + constant GPIO_OUT_SET 
SIO_BASE $018 + constant GPIO_OUT_CLR 
SIO_BASE $01c + constant GPIO_OUT_XOR

25 constant ONBOARD_LED

\ transform GPIO number in his binary mask
: PIN_MASK ( n -- mask )
    1 swap lshift
  ;

\ toggle led
: led.toggle ( gpio -- )
    PIN_MASK GPIO_OUT_XOR !
  ;

: blink ( -- )
    5 $400140CC !
    ONBOARD_LED PIN_MASK GPIO_OE_SET !
    begin
        ONBOARD_LED led.toggle
        300 ms
    key? until
  ;


\ *** BLINK version with gpio_set_dir ******************************************

$D0000000 constant SIO_BASE
SIO_BASE $020 + constant GPIO_OE
SIO_BASE $024 + constant GPIO_OE_SET
SIO_BASE $028 + constant GPIO_OE_CLR

SIO_BASE $010 + constant GPIO_OUT
SIO_BASE $014 + constant GPIO_OUT_SET 
SIO_BASE $018 + constant GPIO_OUT_CLR 
SIO_BASE $01c + constant GPIO_OUT_XOR

25 constant ONBOARD_LED

\ transform GPIO number in his binary mask
: PIN_MASK ( n -- mask )
    1 swap lshift
  ;

1 constant GPIO_OUT
0 constant GPIO_IN

\ set direction for selected gpio
: gpio_set_dir  ( gpio state -- )
    if     PIN_MASK GPIO_OE_SET !
    else   PIN_MASK GPIO_OE_CLR !    then
  ;

\ toggle led
: led.toggle ( gpio -- )
    PIN_MASK GPIO_OUT_XOR !
  ;

: blink ( -- )
    5 $400140CC !
    ONBOARD_LED GPIO_OUT gpio_set_dir
    begin
        ONBOARD_LED led.toggle
        300 ms
    key? until
  ;


\ *** BLINK version with gpio_put **********************************************

$D0000000 constant SIO_BASE
SIO_BASE $020 + constant GPIO_OE
SIO_BASE $024 + constant GPIO_OE_SET
SIO_BASE $028 + constant GPIO_OE_CLR

SIO_BASE $010 + constant GPIO_OUT
SIO_BASE $014 + constant GPIO_OUT_SET 
SIO_BASE $018 + constant GPIO_OUT_CLR 
SIO_BASE $01c + constant GPIO_OUT_XOR

25 constant ONBOARD_LED

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

\ toggle led
: led.toggle ( gpio -- )
    PIN_MASK GPIO_OUT_XOR !
  ;

: blink ( -- )
    5 $400140CC !
    ONBOARD_LED GPIO_OUT gpio_set_dir
    begin
        ONBOARD_LED led.toggle
        300 ms
    key? until
  ;


/ *** BLINK version with gpio_set_function *************************************

$D0000000 constant SIO_BASE
SIO_BASE $020 + constant GPIO_OE
SIO_BASE $024 + constant GPIO_OE_SET
SIO_BASE $028 + constant GPIO_OE_CLR

SIO_BASE $010 + constant GPIO_OUT
SIO_BASE $014 + constant GPIO_OUT_SET 
SIO_BASE $018 + constant GPIO_OUT_CLR 
SIO_BASE $01c + constant GPIO_OUT_XOR

25 constant ONBOARD_LED

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

\ toggle led
: led.toggle ( gpio -- )
    PIN_MASK GPIO_OUT_XOR !
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

: blink ( -- )
    ONBOARD_LED GPIO_FUNC_SIO gpio_set_function
    ONBOARD_LED GPIO_OUT gpio_set_dir
    begin
        ONBOARD_LED led.toggle
        300 ms
    key? until
  ;




