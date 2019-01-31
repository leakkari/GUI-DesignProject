//****************************************************************************************************
//
// Karim El Hallaoui - 260481131
//
// Initilizes all peripherals:
//	- The GPIO for all inputs and outputs for the alpha numeric keypad, 7 segment display and LEDs.
//****************************************************************************************************
#include <stdio.h>
#include "stm32f4xx.h"                  // Device header
#include "stm32f4xx_conf.h"
#include "initialization.h"

// Initilizes the structures to be configured
GPIO_InitTypeDef   GPIO_InitStructure;
//EXTI_InitTypeDef   EXTI_InitStructure;

void initialize_outputs()																			// Initilizes the output pins for on board LEDs
{
	RCC_AHB1PeriphClockCmd (RCC_AHB1Periph_GPIOA, ENABLE); 	// Enables the AHB1 peripheral clock, providing power to GPIOA branch
	GPIO_InitStructure.GPIO_Pin = GPIO_Pin_2 | GPIO_Pin_3 | GPIO_Pin_8 | GPIO_Pin_15; // Select the following pins to initilise
	GPIO_InitStructure.GPIO_Mode = GPIO_Mode_OUT; 					// Specifies the operating mode for the selected pins, sets pins as outputs
	GPIO_InitStructure.GPIO_Speed = GPIO_Speed_100MHz; 			// Don't limit slew rate, allow values to change as fast as they are set
	GPIO_InitStructure.GPIO_OType = GPIO_OType_PP; 					// Specifies the operating output type for the selected pins
	GPIO_InitStructure.GPIO_PuPd = GPIO_PuPd_NOPULL; 				// Pull-up/Pull down for the selected pins, since no input don't pull
	GPIO_Init(GPIOA, &GPIO_InitStructure); 									// Initializes the GPIOD peripheral according to the specified parameters in the GPIO_InitStruct

	RCC_AHB1PeriphClockCmd (RCC_AHB1Periph_GPIOB, ENABLE); 	// Enables the AHB1 peripheral clock, providing power to GPIOD branch
	GPIO_InitStructure.GPIO_Pin = GPIO_Pin_0 | GPIO_Pin_1 | GPIO_Pin_2 | GPIO_Pin_4 | 
																GPIO_Pin_5 | GPIO_Pin_7 | GPIO_Pin_8 | GPIO_Pin_11 | 
																GPIO_Pin_12 | GPIO_Pin_13 | GPIO_Pin_14 | GPIO_Pin_15; // Select the following pins to initilise
	GPIO_InitStructure.GPIO_Mode = GPIO_Mode_OUT; 					// Specifies the operating mode for the selected pins, sets pins as outputs
	GPIO_InitStructure.GPIO_Speed = GPIO_Speed_100MHz; 			// Don't limit slew rate, allow values to change as fast as they are set
	GPIO_InitStructure.GPIO_OType = GPIO_OType_PP; 					// Specifies the operating output type for the selected pins
	GPIO_InitStructure.GPIO_PuPd = GPIO_PuPd_NOPULL; 				// Pull-up/Pull down for the selected pins, since no input don't pull
	GPIO_Init(GPIOB, &GPIO_InitStructure); 									// Initializes the GPIOD peripheral according to the specified parameters in the GPIO_InitStruct

	RCC_AHB1PeriphClockCmd (RCC_AHB1Periph_GPIOC, ENABLE); 	// Enables the AHB1 peripheral clock, providing power to GPIOD branch
	GPIO_InitStructure.GPIO_Pin = GPIO_Pin_1 | GPIO_Pin_5 | 
																GPIO_Pin_6 | GPIO_Pin_8 | GPIO_Pin_9 | GPIO_Pin_11 | 
																GPIO_Pin_13; // Select the following pins to initilise
	GPIO_InitStructure.GPIO_Mode = GPIO_Mode_OUT; 					// Specifies the operating mode for the selected pins, sets pins as outputs
	GPIO_InitStructure.GPIO_Speed = GPIO_Speed_100MHz; 			// Don't limit slew rate, allow values to change as fast as they are set
	GPIO_InitStructure.GPIO_OType = GPIO_OType_PP; 					// Specifies the operating output type for the selected pins
	GPIO_InitStructure.GPIO_PuPd = GPIO_PuPd_NOPULL; 				// Pull-up/Pull down for the selected pins, since no input don't pull
	GPIO_Init(GPIOC, &GPIO_InitStructure); 									// Initializes the GPIOD peripheral according to the specified parameters in the GPIO_InitStruct

	RCC_AHB1PeriphClockCmd (RCC_AHB1Periph_GPIOD, ENABLE); 	// Enables the AHB1 peripheral clock, providing power to GPIOD branch
	GPIO_InitStructure.GPIO_Pin = GPIO_Pin_0 | GPIO_Pin_1 | GPIO_Pin_2 | GPIO_Pin_3 | 
																GPIO_Pin_6 | GPIO_Pin_7 | GPIO_Pin_8 | GPIO_Pin_9 | GPIO_Pin_10 | GPIO_Pin_11 | 
																GPIO_Pin_12 | GPIO_Pin_13 | GPIO_Pin_14 | GPIO_Pin_15; // Select the following pins to initilise
	GPIO_InitStructure.GPIO_Mode = GPIO_Mode_OUT; 					// Specifies the operating mode for the selected pins, sets pins as outputs
	GPIO_InitStructure.GPIO_Speed = GPIO_Speed_100MHz; 			// Don't limit slew rate, allow values to change as fast as they are set
	GPIO_InitStructure.GPIO_OType = GPIO_OType_PP; 					// Specifies the operating output type for the selected pins
	GPIO_InitStructure.GPIO_PuPd = GPIO_PuPd_NOPULL; 				// Pull-up/Pull down for the selected pins, since no input don't pull
	GPIO_Init(GPIOD, &GPIO_InitStructure); 									// Initializes the GPIOD peripheral according to the specified parameters in the GPIO_InitStruct

	RCC_AHB1PeriphClockCmd (RCC_AHB1Periph_GPIOE, ENABLE); 	// Enables the AHB1 peripheral clock, providing power to GPIOD branch
	GPIO_InitStructure.GPIO_Pin = GPIO_Pin_3 | GPIO_Pin_4 | GPIO_Pin_5 |
																GPIO_Pin_6 | GPIO_Pin_7 | GPIO_Pin_8 | GPIO_Pin_9 | GPIO_Pin_10 | GPIO_Pin_11 | 
																GPIO_Pin_12 | GPIO_Pin_13 | GPIO_Pin_14 | GPIO_Pin_15; // Select the following pins to initilise
	GPIO_InitStructure.GPIO_Mode = GPIO_Mode_OUT; 					// Specifies the operating mode for the selected pins, sets pins as outputs
	GPIO_InitStructure.GPIO_Speed = GPIO_Speed_100MHz; 			// Don't limit slew rate, allow values to change as fast as they are set
	GPIO_InitStructure.GPIO_OType = GPIO_OType_PP; 					// Specifies the operating output type for the selected pins
	GPIO_InitStructure.GPIO_PuPd = GPIO_PuPd_NOPULL; 				// Pull-up/Pull down for the selected pins, since no input don't pull
	GPIO_Init(GPIOE, &GPIO_InitStructure); 									// Initializes the GPIOD peripheral according to the specified parameters in the GPIO_InitStruct

	GPIO_InitStructure.GPIO_Pin = GPIO_Pin_4 | GPIO_Pin_2; // Select the following pins to initilise
	GPIO_InitStructure.GPIO_Mode = GPIO_Mode_IN; 					// Specifies the operating mode for the selected pins, sets pins as outputs
	GPIO_InitStructure.GPIO_Speed = GPIO_Speed_100MHz; 			// Don't limit slew rate, allow values to change as fast as they are set
	GPIO_InitStructure.GPIO_OType = GPIO_OType_PP; 					// Specifies the operating output type for the selected pins
	GPIO_InitStructure.GPIO_PuPd = GPIO_PuPd_NOPULL; 				// Pull-up/Pull down for the selected pins, since no input don't pull
	GPIO_Init(GPIOC, &GPIO_InitStructure); 									// Initializes the GPIOD peripheral according to the specified parameters in the GPIO_InitStruct

}
