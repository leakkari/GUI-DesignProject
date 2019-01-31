//****************************************************************************************************
//
// Karim El Hallaoui - 260481131
//
//****************************************************************************************************
#include <stdio.h>
#include "stm32f4xx.h"                  // Device header
#include "stm32f4xx_conf.h"

#ifndef outputs_H_
#define outputs_H_

//void set_bits_tx(int Tx);			// Turns on Tx
//void set_bits_rx(int Rx);			// Turns on Rx
void set_bits(int tx, int rx);
void toggle_LED(void);					// Toggles the LED to shwo switching is occurring

#endif
