//****************************************************************************************************
//
// Karim El Hallaoui - 260481131
// Pragyan Hazarika
//
// Sets the appropriate bits for the Tx and Rx that is selected
//
//****************************************************************************************************

#include <stdio.h>
#include "stm32f4xx.h"                  // Device header
#include "stm32f4xx_conf.h"
#include "outputs.h"

// Set up a struct that defines what bus and what pin each input to the switching matrix is associated to
struct H_P{
		GPIO_TypeDef* bus;
		uint16_t pin; 
};

int LED_flag = 0;

// States for the transmitting antenna.
int tx_states[16][10] = {{0,0,0,0,0,0,0,0,0,0}, // TX 
												 {0,0,1,0,1,0,1,0,1,0},
												 {0,0,0,1,0,1,0,1,0,1},
												 {0,0,1,1,1,1,1,1,1,1},
												 
												 {1,0,0,0,0,0,0,0,0,0},												 
												 {1,0,1,0,1,0,1,0,1,0},
												 {1,0,0,1,0,1,0,1,0,1},
												 {1,0,1,1,1,1,1,1,1,1},
												 
												 {0,1,0,0,0,0,0,0,0,0},												 
												 {0,1,1,0,1,0,1,0,1,0},
												 {0,1,0,1,0,1,0,1,0,1},
												 {0,1,1,1,1,1,1,1,1,1},
												 
												 {1,1,0,0,0,0,0,0,0,0},												 
												 {1,1,1,0,1,0,1,0,1,0},
												 {1,1,0,1,0,1,0,1,0,1},
												 {1,1,1,1,1,1,1,1,1,1}}; 

// States for the receiving antenna.
int rx_states[16][10] = {{0,0,0,0,0,0,0,0,0,0}, // RX
												 {0,0,1,0,1,0,1,0,1,0},
												 {0,0,0,1,0,1,0,1,0,1},
												 {0,0,1,1,1,1,1,1,1,1},
												 
												 {1,0,0,0,0,0,0,0,0,0},
												 {1,0,1,0,1,0,1,0,1,0},
												 {1,0,0,1,0,1,0,1,0,1},
												 {1,0,1,1,1,1,1,1,1,1},
												 
												 {0,1,0,0,0,0,0,0,0,0},
												 {0,1,1,0,1,0,1,0,1,0},
												 {0,1,0,1,0,1,0,1,0,1},
												 {0,1,1,1,1,1,1,1,1,1},
												 
												 {1,1,0,0,0,0,0,0,0,0},
												 {1,1,1,0,1,0,1,0,1,0},
												 {1,1,0,1,0,1,0,1,0,1},
												 {1,1,1,1,1,1,1,1,1,1}}; 

struct H_P pins[36] = { {GPIOE,GPIO_Pin_9},{GPIOE,GPIO_Pin_10},{GPIOE,GPIO_Pin_14}, {GPIOB,GPIO_Pin_11},
                        {GPIOD,GPIO_Pin_9}, {GPIOD,GPIO_Pin_8}, {GPIOE,GPIO_Pin_6},{GPIOC,GPIO_Pin_13},
                        {GPIOB,GPIO_Pin_0},{GPIOB,GPIO_Pin_2},

                        {GPIOE,GPIO_Pin_11}, {GPIOE,GPIO_Pin_12}, {GPIOB,GPIO_Pin_12},{GPIOB,GPIO_Pin_13}, 
                        {GPIOB,GPIO_Pin_14}, {GPIOB,GPIO_Pin_15}, {GPIOD,GPIO_Pin_11},{GPIOD,GPIO_Pin_10},
                        {GPIOE,GPIO_Pin_7}, {GPIOE,GPIO_Pin_8}, 
														 
                        {GPIOB,GPIO_Pin_4},{GPIOB,GPIO_Pin_7}, {GPIOD,GPIO_Pin_3}, {GPIOD,GPIO_Pin_7},
												{GPIOD,GPIO_Pin_2}, {GPIOD,GPIO_Pin_1}, {GPIOC,GPIO_Pin_11}, {GPIOD,GPIO_Pin_0},
                        {GPIOC,GPIO_Pin_8},{GPIOA,GPIO_Pin_15}, {GPIOC,GPIO_Pin_6}, {GPIOC,GPIO_Pin_9},
                        {GPIOE,GPIO_Pin_5}, {GPIOE,GPIO_Pin_4}, {GPIOB,GPIO_Pin_6}, {GPIOE,GPIO_Pin_3}
                       }; 

void set_bits(int tx, int rx)
{
	int j;
	int i = 0;
	int pin_ix = 0;
		
	// Set the necessary TX bits														
	i=0;													
	for ( i=0; i<10; i++ ) {
		if(tx_states[tx][i] == 0){												
			GPIO_ResetBits( pins[pin_ix].bus, pins[pin_ix].pin );
		}
		else if(tx_states[tx][i] == 1){
			GPIO_SetBits( pins[pin_ix].bus, pins[pin_ix].pin );
		}
		pin_ix++;
	}
	
	// Set the necessary RX bits
	i = 0;
	for ( i=0; i<10; i++ ) {
		if(rx_states[rx][i] == 0){												
			GPIO_ResetBits( pins[pin_ix].bus, pins[pin_ix].pin );
		}
		else if(rx_states[rx][i] == 1){
			GPIO_SetBits( pins[pin_ix].bus, pins[pin_ix].pin );
		}
		pin_ix++;
	}
	
	// Set antenna bits
	i=0;													
	for ( i=0; i<16; i++ ) {
		if(i == tx){
			GPIO_ResetBits( pins[pin_ix].bus, pins[pin_ix].pin );
		}
		else{
			GPIO_SetBits( pins[pin_ix].bus, pins[pin_ix].pin );
		}
		pin_ix++;
	}
	
	toggle_LED();
}


// Function used to toggle the LEDs at the specified pins
// This is a visual cue to show switching is occuring
void toggle_LED(void)
{
	if(LED_flag==0)
	{
		GPIO_ResetBits(GPIOD, GPIO_Pin_12);
		GPIO_ResetBits(GPIOD, GPIO_Pin_14);
		GPIO_SetBits(GPIOD, GPIO_Pin_13);
		GPIO_SetBits(GPIOD, GPIO_Pin_15);
		LED_flag = 1;
	}
	else
	{
		GPIO_ResetBits(GPIOD, GPIO_Pin_13);
		GPIO_ResetBits(GPIOD, GPIO_Pin_15);
		GPIO_SetBits(GPIOD, GPIO_Pin_12);
		GPIO_SetBits(GPIOD, GPIO_Pin_14);
		LED_flag = 0;
	}
}
