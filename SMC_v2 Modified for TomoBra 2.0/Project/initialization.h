//****************************************************************************************************
// ECSE 426 Microprocessor Systems
// Lab 3: MEMS Accelerometer and Guess the Angle Game
//
// Karim El Hallaoui - 260481131
// Sacha Terzian - 260481022
//
// Initilizes all peripherals:
//	- MEMS accelerometer and its interrupt line
//	- NVIC interrupt priorities
//	- The internal hardware timer (TIM) and its interrupt
//	- The GPIO for all inputs and outputs for the alpha numeric keypad, 7 segment display and LEDs.
//****************************************************************************************************

#include <stdio.h>
#include "stm32f4xx.h"                 // Device header
#include "stm32f4xx_conf.h"

#ifndef initialization_H_
#define initialization_H_

void initialize_outputs(void); 						// Initilizes the output pins for on board LEDs	

#endif

