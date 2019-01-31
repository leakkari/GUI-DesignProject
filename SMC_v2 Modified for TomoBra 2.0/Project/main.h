  
/* Define to prevent recursive inclusion -------------------------------------*/
#ifndef __STM32F4_DISCOVERY_HIDEX_H
#define __STM32F4_DISCOVERY_HIDEX_H

/* Includes ------------------------------------------------------------------*/
#include "stm32f4_discovery.h"

#include <stdio.h>

/* Exported types ------------------------------------------------------------*/
/* Exported constants --------------------------------------------------------*/


/* Exported macro ------------------------------------------------------------*/
#define ABS(x)         (x < 0) ? (-x) : x
#define MAX(a,b)       (a < b) ? (b) : a
/* Exported functions ------------------------------------------------------- */
void TimingDelay_Decrement(void);
void Delay(__IO uint32_t nTime);
void Fail_Handler(void);
#endif /* __STM32F4_DISCOVERY_HIDEX_H */

/******************* (C) COPYRIGHT 2011 STMicroelectronics *****END OF FILE****/
