/**
  ******************************************************************************
  * @file    usbd_hid_core.c
  * @author  MCD Application Team
  * @version V1.0.0
  * @date    22-July-2011
  * @brief   This file provides the HID core functions.
  *
  * @verbatim
  *      
  *          ===================================================================      
  *                                HID Class  Description
  *          =================================================================== 
  *           This module manages the HID class V1.11 following the "Device Class Definition
  *           for Human Interface Devices (HID) Version 1.11 Jun 27, 2001".
  *           This driver implements the following aspects of the specification:
  *             - The Boot Interface Subclass
  *             - The Custom Generic protocol
  *             - Usage Page : Generic Desktop
  *             - Collection : Application 
  *      
  * @note     In HS mode and when the DMA is used, all variables and data structures
  *           dealing with the DMA during the transaction process should be 32-bit aligned.
  *           
  *      
  *  @endverbatim
  *
  ******************************************************************************
  * @attention
  *
  * THE PRESENT FIRMWARE WHICH IS FOR GUIDANCE ONLY AIMS AT PROVIDING CUSTOMERS
  * WITH CODING INFORMATION REGARDING THEIR PRODUCTS IN ORDER FOR THEM TO SAVE
  * TIME. AS A RESULT, STMICROELECTRONICS SHALL NOT BE HELD LIABLE FOR ANY
  * DIRECT, INDIRECT OR CONSEQUENTIAL DAMAGES WITH RESPECT TO ANY CLAIMS ARISING
  * FROM THE CONTENT OF SUCH FIRMWARE AND/OR THE USE MADE BY CUSTOMERS OF THE
  * CODING INFORMATION CONTAINED HEREIN IN CONNECTION WITH THEIR PRODUCTS.
  *
  * <h2><center>&copy; COPYRIGHT 2011 STMicroelectronics</center></h2>
  ******************************************************************************
  */ 

/* Includes ------------------------------------------------------------------*/
#include "usbd_hid_core.h"
#include "usbd_desc.h"
#include "usbd_req.h"


/** @addtogroup STM32_USB_OTG_DEVICE_LIBRARY
  * @{
  */


/** @defgroup USBD_HID 
  * @brief usbd core module
  * @{
  */ 

/** @defgroup USBD_HID_Private_TypesDefinitions
  * @{
  */ 
/**
  * @}
  */ 


/** @defgroup USBD_HID_Private_Defines
  * @{
  */ 

/**
  * @}
  */ 


/** @defgroup USBD_HID_Private_Macros
  * @{
  */ 
/**
  * @}
  */ 




/** @defgroup USBD_HID_Private_FunctionPrototypes
  * @{
  */


static uint8_t  USBD_HID_Init (void  *pdev, 
                               uint8_t cfgidx);

static uint8_t  USBD_HID_DeInit (void  *pdev, 
                                 uint8_t cfgidx);

static uint8_t  USBD_HID_Setup (void  *pdev, 
                                USB_SETUP_REQ *req);

static uint8_t  *USBD_HID_GetCfgDesc (uint8_t speed, uint16_t *length);

static uint8_t  USBD_HID_DataIn (void  *pdev, uint8_t epnum);

static uint8_t  USBD_HID_DataOut (void  *pdev, uint8_t epnum);


/**
  * @}
  */ 

/** @defgroup USBD_HID_Private_Variables
  * @{
  */ 

USBD_Class_cb_TypeDef  USBD_HID_cb = 
{
  USBD_HID_Init,
  USBD_HID_DeInit,
  USBD_HID_Setup,
  NULL, /*EP0_TxSent*/  
  NULL, /*EP0_RxReady*/
  USBD_HID_DataIn, /*DataIn*/
  USBD_HID_DataOut, /*DataOut*/
  NULL, /*SOF */
  NULL,
  NULL,      
  USBD_HID_GetCfgDesc,
#ifdef USB_OTG_HS_CORE  
  USBD_HID_GetCfgDesc, /* use same config as per FS */
#endif  
};

#ifdef USB_OTG_HS_INTERNAL_DMA_ENABLED
  #if defined ( __ICCARM__ ) /*!< IAR Compiler */
    #pragma data_alignment=4   
  #endif
#endif /* USB_OTG_HS_INTERNAL_DMA_ENABLED */        
__ALIGN_BEGIN static uint32_t  USBD_HID_AltSet  __ALIGN_END = 0;

#ifdef USB_OTG_HS_INTERNAL_DMA_ENABLED
  #if defined ( __ICCARM__ ) /*!< IAR Compiler */
    #pragma data_alignment=4   
  #endif
#endif /* USB_OTG_HS_INTERNAL_DMA_ENABLED */      
__ALIGN_BEGIN static uint32_t  USBD_HID_Protocol  __ALIGN_END = 0;

#ifdef USB_OTG_HS_INTERNAL_DMA_ENABLED
  #if defined ( __ICCARM__ ) /*!< IAR Compiler */
    #pragma data_alignment=4   
  #endif
#endif /* USB_OTG_HS_INTERNAL_DMA_ENABLED */  
__ALIGN_BEGIN static uint32_t  USBD_HID_IdleState __ALIGN_END = 0;

#ifdef USB_OTG_HS_INTERNAL_DMA_ENABLED
  #if defined ( __ICCARM__ ) /*!< IAR Compiler */
    #pragma data_alignment=4   
  #endif
#endif /* USB_OTG_HS_INTERNAL_DMA_ENABLED */ 
/* USB HID device Configuration Descriptor */
__ALIGN_BEGIN static uint8_t USBD_HID_CfgDesc[USB_HID_CONFIG_DESC_SIZ] __ALIGN_END =
{
  0x09, /* bLength: Configuration Descriptor size */
  USB_CONFIGURATION_DESCRIPTOR_TYPE, /* bDescriptorType: Configuration */
  USB_HID_CONFIG_DESC_SIZ,
  /* wTotalLength: Bytes returned */
  0x00,
  0x01,         /*bNumInterfaces: 1 interface*/
  0x01,         /*bConfigurationValue: Configuration value*/
  0x00,         /*iConfiguration: Index of string descriptor describing
  the configuration*/
  0xE0,         /*bmAttributes: bus powered and Support Remote Wake-up */
  0x32,         /*MaxPower 100 mA: this current is used for detecting Vbus*/
  

  /************** Descriptor of Custom HID interface ****************/
  /* 09 */
  0x09,         /* bLength: Interface Descriptor size */
  USB_INTERFACE_DESCRIPTOR_TYPE, /* bDescriptorType: Interface descriptor type */
  0x00,         /* bInterfaceNumber: Number of Interface */
  0x00,         /* bAlternateSetting: Alternate setting */
  0x02,         /* bNumEndpoints */
  0x03,         /* bInterfaceClass: HID */
  0x00,         /* bInterfaceSubClass : 1=BOOT, 0=no boot */
  0x00,         /* nInterfaceProtocol : 0=none, 1=keyboard, 2=mouse */
  0,            /* iInterface: Index of string descriptor */
  /******************** Descriptor of Custom HID ********************/
  /* 18 */
 0x09,         /* bLength: HID Descriptor size */
  HID_DESCRIPTOR_TYPE, /* bDescriptorType: HID */
  0x10,         /* bcdHID: HID Class Spec release number ?? 0x11 ??*/
  0x01,
  0x00,         /* bCountryCode: Hardware target country */
  0x01,         /* bNumDescriptors: Number of HID class descriptors to follow */
  0x22,         /* bDescriptorType */
  CUSTOMHID_SIZ_REPORT_DESC_L,/* wItemLength: Total length of Report descriptor */
  CUSTOMHID_SIZ_REPORT_DESC_H,
  /******************** Descriptor of Custom HID endpoints ******************/
  /* 27 */
  0x07,          /* bLength: Endpoint Descriptor size */
  USB_ENDPOINT_DESCRIPTOR_TYPE, /* bDescriptorType: */

  HID_IN_EP,     /* bEndpointAddress: Endpoint Address (IN) */
  0x03,          /* bmAttributes: Interrupt endpoint */
  HID_IN_PACKET, /* wMaxPacketSize */
  0x00,
  0x0A,          /* bInterval: Polling Interval (10 ms) */
  /* 34 */

  0x07,  	/* bLength: Endpoint Descriptor size */
  USB_ENDPOINT_DESCRIPTOR_TYPE,	/* bDescriptorType: */

  HID_OUT_EP,	/* bEndpointAddress: Endpoint Address (OUT) */
  0x03,			/* bmAttributes: Interrupt endpoint */
  HID_OUT_PACKET,	/* wMaxPacketSize */
  0x00,
  0x0A,			/* bInterval: Polling Interval (10 ms) */
  /* 41 */
}
; /* CustomHID_ConfigDescriptor */


#ifdef USB_OTG_HS_INTERNAL_DMA_ENABLED
  #if defined ( __ICCARM__ ) /*!< IAR Compiler */
    #pragma data_alignment=4   
  #endif
#endif /* USB_OTG_HS_INTERNAL_DMA_ENABLED */  

__ALIGN_BEGIN static uint8_t CustomHID_ReportDescriptor[CUSTOMHID_SIZ_REPORT_DESC] __ALIGN_END =
{
  0x06, 0xFF, 0x00,      /* USAGE_PAGE (Vendor Page: 0xFF00) */
  0x09, 0x01,            /* USAGE (custom) */
  0xA1, 0x01,            /* COLLECTION (Application)       */
  /* 6 */
  
  /* Switch #1 */
  0x85, 0x01,            /*     REPORT_ID (1)		     */
  0x09, 0x01,            /*     USAGE (SW1)	             */
  0x15, 0x00,            /*     LOGICAL_MINIMUM (0)        */
  0x25, 0x07,            /*     LOGICAL_MAXIMUM (7)        */
  0x75, 0x08,            /*     REPORT_SIZE (8)            */
  0x95, 0x01,            /*     REPORT_COUNT (1)           */
  0xB1, 0x82,             /*    FEATURE (Data,Var,Abs,Vol) */

  0x85, 0x01,            /*     REPORT_ID (1)              */
  0x09, 0x01,            /*     USAGE (SW1)              */
  0x91, 0x82,            /*     OUTPUT (Data,Var,Abs,Vol)  */
  /* 26 */
  
  /* SW2 */
  0x85, 0x02,            /*     REPORT_ID 2		     */
  0x09, 0x02,            /*     USAGE (SW2)	             */
  0x15, 0x00,            /*     LOGICAL_MINIMUM (0)        */
  0x25, 0x07,            /*     LOGICAL_MAXIMUM (7)        */
  0x75, 0x08,            /*     REPORT_SIZE (8)            */
  0x95, 0x01,            /*     REPORT_COUNT (1)           */
  0xB1, 0x82,             /*    FEATURE (Data,Var,Abs,Vol) */

  0x85, 0x02,            /*     REPORT_ID (2)              */
  0x09, 0x02,            /*     USAGE (SW2)              */
  0x91, 0x82,            /*     OUTPUT (Data,Var,Abs,Vol)  */
  /* 46 */
  
  /* SW3 */
  0x85, 0x03,            /*     REPORT_ID (3)		     */
  0x09, 0x03,            /*     USAGE (SW3)	             */
  0x15, 0x00,            /*     LOGICAL_MINIMUM (0)        */
  0x25, 0x03,            /*     LOGICAL_MAXIMUM (3)        */
  0x75, 0x08,            /*     REPORT_SIZE (8)            */
  0x95, 0x01,            /*     REPORT_COUNT (1)           */
  0xB1, 0x82,             /*    FEATURE (Data,Var,Abs,Vol) */

  0x85, 0x03,            /*     REPORT_ID (3)              */
  0x09, 0x03,            /*     USAGE (SW3)              */
  0x91, 0x82,            /*     OUTPUT (Data,Var,Abs,Vol)  */
  /* 66 */
  
  /* SW4 */
  0x85, 0x04,            /*     REPORT_ID (4)		     */
  0x09, 0x04,            /*     USAGE (SW4)	             */
  0x15, 0x00,            /*     LOGICAL_MINIMUM (0)        */
  0x25, 0x07,            /*     LOGICAL_MAXIMUM (7)        */
  0x75, 0x08,            /*     REPORT_SIZE (8)            */
  0x95, 0x01,            /*     REPORT_COUNT (1)           */
  0xB1, 0x82,            /*     FEATURE (Data,Var,Abs,Vol) */

  0x85, 0x04,            /*     REPORT_ID (4)              */
  0x09, 0x04,            /*     USAGE (SW4)              */
  0x91, 0x82,            /*     OUTPUT (Data,Var,Abs,Vol)  */
  /* 86 */
  
  /* SW5 */
  0x85, 0x05,            /*     REPORT_ID (5)		     */
  0x09, 0x05,            /*     USAGE (SW5)	             */
  0x15, 0x00,            /*     LOGICAL_MINIMUM (0)        */
  0x25, 0x07,            /*     LOGICAL_MAXIMUM (7)        */
  0x75, 0x08,            /*     REPORT_SIZE (8)            */
  0x95, 0x01,            /*     REPORT_COUNT (1)           */
  0xB1, 0x82,            /*     FEATURE (Data,Var,Abs,Vol) */

  0x85, 0x05,            /*     REPORT_ID (5)              */
  0x09, 0x05,            /*     USAGE (SW5)              */
  0x91, 0x82,            /*     OUTPUT (Data,Var,Abs,Vol)  */
  /* 106 */

  /* SW6 */
  0x85, 0x06,            /*     REPORT_ID (6)		     */
  0x09, 0x06,            /*     USAGE (SW6)	             */
  0x15, 0x00,            /*     LOGICAL_MINIMUM (0)        */
  0x25, 0x03,            /*     LOGICAL_MAXIMUM (3)        */
  0x75, 0x08,            /*     REPORT_SIZE (8)            */
  0x95, 0x01,            /*     REPORT_COUNT (1)           */
  0xB1, 0x82,             /*    FEATURE (Data,Var,Abs,Vol) */

  0x85, 0x06,            /*     REPORT_ID (6)              */
  0x09, 0x06,            /*     USAGE (SW6)              */
  0x91, 0x82,            /*     OUTPUT (Data,Var,Abs,Vol)  */
  /* 126 */

  /* SW7 */
  0x85, 0x07,            /*     REPORT_ID (7)		     */
  0x09, 0x07,            /*     USAGE (SW7)	             */
  0x15, 0x00,            /*     LOGICAL_MINIMUM (0)        */
  0x25, 0x07,            /*     LOGICAL_MAXIMUM (7)        */
  0x75, 0x08,            /*     REPORT_SIZE (8)            */
  0x95, 0x01,            /*     REPORT_COUNT (1)           */
  0xB1, 0x82,            /*     FEATURE (Data,Var,Abs,Vol) */

  0x85, 0x07,            /*     REPORT_ID (7)              */
  0x09, 0x07,            /*     USAGE (SW7)              */
  0x91, 0x82,            /*     OUTPUT (Data,Var,Abs,Vol)  */
  /* 146 */

  /* SW8 */
  0x85, 0x08,            /*     REPORT_ID (8)		     */
  0x09, 0x08,            /*     USAGE (SW8)	             */
  0x15, 0x00,            /*     LOGICAL_MINIMUM (0)        */
  0x25, 0x07,            /*     LOGICAL_MAXIMUM (7)        */
  0x75, 0x08,            /*     REPORT_SIZE (8)            */
  0x95, 0x01,            /*     REPORT_COUNT (1)           */
  0xB1, 0x82,            /*     FEATURE (Data,Var,Abs,Vol) */

  0x85, 0x08,            /*     REPORT_ID (8)              */
  0x09, 0x08,            /*     USAGE (SW8)              */
  0x91, 0x82,            /*     OUTPUT (Data,Var,Abs,Vol)  */
  /* 166 */

  /* SW9 */
  0x85, 0x09,            /*     REPORT_ID (9)		     */
  0x09, 0x09,            /*     USAGE (SW9)	             */
  0x15, 0x00,            /*     LOGICAL_MINIMUM (0)        */
  0x25, 0x07,            /*     LOGICAL_MAXIMUM (7)        */
  0x75, 0x08,            /*     REPORT_SIZE (8)            */
  0x95, 0x01,            /*     REPORT_COUNT (1)           */
  0xB1, 0x82,            /*     FEATURE (Data,Var,Abs,Vol) */

  0x85, 0x09,            /*     REPORT_ID (9)              */
  0x09, 0x09,            /*     USAGE (SW9)              */
  0x91, 0x82,            /*     OUTPUT (Data,Var,Abs,Vol)  */
  /* 186 */
	
	/* SW10 */
  0x85, 0x0A,            /*     REPORT_ID (10)		     */
  0x09, 0x0A,            /*     USAGE (SW10)	             */
  0x15, 0x00,            /*     LOGICAL_MINIMUM (0)        */
  0x25, 0x07,            /*     LOGICAL_MAXIMUM (7)        */
  0x75, 0x08,            /*     REPORT_SIZE (8)            */
  0x95, 0x01,            /*     REPORT_COUNT (1)           */
  0xB1, 0x82,            /*     FEATURE (Data,Var,Abs,Vol) */

  0x85, 0x0A,            /*     REPORT_ID (10)              */
  0x09, 0x0A,            /*     USAGE (SW10)              */
  0x91, 0x82,            /*     OUTPUT (Data,Var,Abs,Vol)  */
  /* 206 */

	/* MUX16A */
  0x85, 0x0B,            /*     REPORT_ID (11)		     */
  0x09, 0x0B,            /*     USAGE (MUX16A)	             */
  0x15, 0x00,            /*     LOGICAL_MINIMUM (0)        */
  0x25, 0x0F,            /*     LOGICAL_MAXIMUM (15)        */
  0x75, 0x08,            /*     REPORT_SIZE (8)            */
  0x95, 0x01,            /*     REPORT_COUNT (1)           */
  0xB1, 0x82,            /*     FEATURE (Data,Var,Abs,Vol) */

  0x85, 0x0B,            /*     REPORT_ID (11)              */
  0x09, 0x0B,            /*     USAGE (MUX16A)              */
  0x91, 0x82,            /*     OUTPUT (Data,Var,Abs,Vol)  */
  /* 226 */

	/* MUX16B */
  0x85, 0x0C,            /*     REPORT_ID (12)		     */
  0x09, 0x0C,            /*     USAGE (MUX16B)	             */
  0x15, 0x00,            /*     LOGICAL_MINIMUM (0)        */
  0x25, 0x0F,            /*     LOGICAL_MAXIMUM (15)        */
  0x75, 0x08,            /*     REPORT_SIZE (8)            */
  0x95, 0x01,            /*     REPORT_COUNT (1)           */
  0xB1, 0x82,            /*     FEATURE (Data,Var,Abs,Vol) */

  0x85, 0x0C,            /*     REPORT_ID (12)              */
  0x09, 0x0C,            /*     USAGE (MUX16B)              */
  0x91, 0x82,            /*     OUTPUT (Data,Var,Abs,Vol)  */
  /* 246 */

	/* SW_EN */
  0x85, 0x0D,            /*     REPORT_ID (13)		     */
  0x09, 0x0D,            /*     USAGE (SW_EN)	             */
  0x15, 0x00,            /*     LOGICAL_MINIMUM (0)        */
  0x25, 0x01,            /*     LOGICAL_MAXIMUM (1)        */
  0x75, 0x08,            /*     REPORT_SIZE (8)            */
  0x95, 0x01,            /*     REPORT_COUNT (1)           */
  0xB1, 0x82,            /*     FEATURE (Data,Var,Abs,Vol) */

  0x85, 0x0D,            /*     REPORT_ID (13)              */
  0x09, 0x0D,            /*     USAGE (SW_EN)              */
  0x91, 0x82,            /*     OUTPUT (Data,Var,Abs,Vol)  */
  /* 266 */
	
	/* PG_CTL */
  0x85, 0x0E,            /*     REPORT_ID (14)		     */
  0x09, 0x0E,            /*     USAGE (PG_CTL)	             */
  0x15, 0x00,            /*     LOGICAL_MINIMUM (0)        */
  0x25, 0x01,            /*     LOGICAL_MAXIMUM (1)        */
  0x75, 0x08,            /*     REPORT_SIZE (8)            */
  0x95, 0x01,            /*     REPORT_COUNT (1)           */
  0xB1, 0x82,            /*     FEATURE (Data,Var,Abs,Vol) */

  0x85, 0x0E,            /*     REPORT_ID (14)              */
  0x09, 0x0E,            /*     USAGE (PG_CTL)              */
  0x91, 0x82,            /*     OUTPUT (Data,Var,Abs,Vol)  */
  /* 286 */
	
	/* POWER_ON */
  0x85, 0x0F,            /*     REPORT_ID (15)		     */
  0x09, 0x0F,            /*     USAGE (POWER_ON)	             */
  0x15, 0x00,            /*     LOGICAL_MINIMUM (0)        */
  0x25, 0x01,            /*     LOGICAL_MAXIMUM (1)        */
  0x75, 0x08,            /*     REPORT_SIZE (8)            */
  0x95, 0x01,            /*     REPORT_COUNT (1)           */
  0xB1, 0x82,            /*     FEATURE (Data,Var,Abs,Vol) */

  0x85, 0x0F,            /*     REPORT_ID (15)              */
  0x09, 0x0F,            /*     USAGE (POWER_ON)              */
  0x91, 0x82,            /*     OUTPUT (Data,Var,Abs,Vol)  */
  /* 306 */
	
	/* POWER_GOOD */
  0x85, 0x10,            /*     REPORT_ID (16)		     */
  0x09, 0x10,            /*     USAGE (POWER_GOOD)	             */
  0x15, 0x00,            /*     LOGICAL_MINIMUM (0)        */
  0x25, 0x01,            /*     LOGICAL_MAXIMUM (1)        */
  0x75, 0x08,            /*     REPORT_SIZE (8)            */
  0x95, 0x01,            /*     REPORT_COUNT (1)           */
  0xB1, 0x82,            /*     FEATURE (Data,Var,Abs,Vol) */

  0x85, 0x10,            /*     REPORT_ID (16)              */
  0x09, 0x10,            /*     USAGE (POWER_GOOD)              */
  0x91, 0x82,            /*     OUTPUT (Data,Var,Abs,Vol)  */
  /* 326 */
	
	/* MUX_EN */
  0x85, 0x11,            /*     REPORT_ID (17)		     */
  0x09, 0x11,            /*     USAGE (MUX_EN)	             */
  0x15, 0x00,            /*     LOGICAL_MINIMUM (0)        */
  0x25, 0x01,            /*     LOGICAL_MAXIMUM (1)        */
  0x75, 0x08,            /*     REPORT_SIZE (8)            */
  0x95, 0x01,            /*     REPORT_COUNT (1)           */
  0xB1, 0x82,            /*     FEATURE (Data,Var,Abs,Vol) */

  0x85, 0x11,            /*     REPORT_ID (17)              */
  0x09, 0x11,            /*     USAGE (MUX_EN)              */
  0x91, 0x82,            /*     OUTPUT (Data,Var,Abs,Vol)  */
  /* 346 */
	
	/* FAN_POWER */
  0x85, 0x12,            /*     REPORT_ID (18)		     		 */
  0x09, 0x12,            /*     USAGE (FAN_POWER)	           */
  0x15, 0x00,            /*     LOGICAL_MINIMUM (0)        */
  0x25, 0x01,            /*     LOGICAL_MAXIMUM (1)        */
  0x75, 0x08,            /*     REPORT_SIZE (8)            */
  0x95, 0x01,            /*     REPORT_COUNT (1)           */
  0xB1, 0x82,            /*     FEATURE (Data,Var,Abs,Vol) */

  0x85, 0x12,            /*     REPORT_ID (18)              */
  0x09, 0x12,            /*     USAGE (FAN_POWER)              */
  0x91, 0x82,            /*     OUTPUT (Data,Var,Abs,Vol)  */
  /* 366 */

	/* SIREN_ON */
  0x85, 0x13,            /*     REPORT_ID (19)		     		 */
  0x09, 0x13,            /*     USAGE (SIREN_ON)	           */
  0x15, 0x00,            /*     LOGICAL_MINIMUM (0)        */
  0x25, 0x01,            /*     LOGICAL_MAXIMUM (1)        */
  0x75, 0x08,            /*     REPORT_SIZE (8)            */
  0x95, 0x01,            /*     REPORT_COUNT (1)           */
  0xB1, 0x82,            /*     FEATURE (Data,Var,Abs,Vol) */

  0x85, 0x13,            /*     REPORT_ID (19)              */
  0x09, 0x13,            /*     USAGE (SIREN_ON)              */
  0x91, 0x82,            /*     OUTPUT (Data,Var,Abs,Vol)  */
  /* 386 */

  0xc0 	          /*     END_COLLECTION	             */
}; /* CustomHID_ReportDescriptor */

extern uint8_t Power_ON;
extern uint8_t Fan_Power;
extern uint8_t SW_EN;
extern uint8_t PG_CTL;
extern uint8_t MUX_EN;
extern uint8_t SW1_Value;
extern uint8_t SW2_Value;
extern uint8_t SW3_Value;
extern uint8_t SW4_Value;
extern uint8_t SW5_Value;
extern uint8_t SW6_Value;
extern uint8_t MUX16A_Value;

/**
  * @}
  */ 

/** @defgroup USBD_HID_Private_Functions
  * @{
  */ 

/**
  * @brief  USBD_HID_Init
  *         Initialize the HID interface
  * @param  pdev: device instance
  * @param  cfgidx: Configuration index
  * @retval status
  */
static uint8_t  USBD_HID_Init (void  *pdev, 
                               uint8_t cfgidx)
{
  
  /* Open EP IN */
  DCD_EP_Open(pdev,
              HID_IN_EP,
              HID_IN_PACKET,
              USB_OTG_EP_INT);
  
  /* Open EP OUT */
  DCD_EP_Open(pdev,
              HID_OUT_EP,
              HID_OUT_PACKET,
              USB_OTG_EP_INT);
  
  /* Prepare Out endpoint to receive next packet */
  DCD_EP_PrepareRx(pdev,
                   HID_OUT_EP,
                   (uint8_t*)(Buffer),
                   HID_OUT_PACKET);
  
  return USBD_OK;
}

/**
  * @brief  USBD_HID_DeInit
  *         DeInitialize the HID layer
  * @param  pdev: device instance
  * @param  cfgidx: Configuration index
  * @retval status
  */
static uint8_t  USBD_HID_DeInit (void  *pdev, 
                                 uint8_t cfgidx)
{
  /* Close HID EPs */
  DCD_EP_Close (pdev , HID_IN_EP);
  DCD_EP_Close (pdev , HID_OUT_EP);
  
  
  return USBD_OK;
}

/**
  * @brief  USBD_HID_Setup
  *         Handle the HID specific requests
  * @param  pdev: instance
  * @param  req: usb requests
  * @retval status
  */
static uint8_t  USBD_HID_Setup (void  *pdev, 
                                USB_SETUP_REQ *req)
{
  uint16_t len = 0;
  uint8_t  *pbuf = NULL;
  
  switch (req->bmRequest & USB_REQ_TYPE_MASK)
  {
  case USB_REQ_TYPE_CLASS :  
    switch (req->bRequest)
    {
      
      
    case HID_REQ_SET_PROTOCOL:
      USBD_HID_Protocol = (uint8_t)(req->wValue);
      break;
      
    case HID_REQ_GET_PROTOCOL:
      USBD_CtlSendData (pdev, 
                        (uint8_t *)&USBD_HID_Protocol,
                        1);    
      break;
      
    case HID_REQ_SET_IDLE:
      USBD_HID_IdleState = (uint8_t)(req->wValue >> 8);
      break;
      
    case HID_REQ_GET_IDLE:
      USBD_CtlSendData (pdev, 
                        (uint8_t *)&USBD_HID_IdleState,
                        1);        
      break;      
      
    default:
      USBD_CtlError (pdev, req);
      return USBD_FAIL; 
    }
    break;
    
  case USB_REQ_TYPE_STANDARD:
    switch (req->bRequest)
    {
    case USB_REQ_GET_DESCRIPTOR: 
      if( req->wValue >> 8 == HID_REPORT_DESC)
      {
        len = MIN(CUSTOMHID_SIZ_REPORT_DESC , req->wLength);
        pbuf = CustomHID_ReportDescriptor;
      }
      else if( req->wValue >> 8 == HID_DESCRIPTOR_TYPE)
      {
        
//#ifdef USB_OTG_HS_INTERNAL_DMA_ENABLED
//        pbuf = USBD_HID_Desc;   
//#else
        pbuf = USBD_HID_CfgDesc + 0x12;
//#endif 
        len = MIN(USB_HID_DESC_SIZ , req->wLength);
      }
      
      USBD_CtlSendData (pdev, 
                        pbuf,
                        len);
      
      break;
      
    case USB_REQ_GET_INTERFACE :
      USBD_CtlSendData (pdev,
                        (uint8_t *)&USBD_HID_AltSet,
                        1);
      break;
      
    case USB_REQ_SET_INTERFACE :
      USBD_HID_AltSet = (uint8_t)(req->wValue);
      break;
    }
  }
  return USBD_OK;
}

/**
  * @brief  USBD_HID_SendReport 
  *         Send HID Report
  * @param  pdev: device instance
  * @param  buff: pointer to report
  * @retval status
  */
uint8_t USBD_HID_SendReport     (USB_OTG_CORE_HANDLE  *pdev, 
                                 uint8_t *report,
                                 uint16_t len)
{
  if (pdev->dev.device_status == USB_OTG_CONFIGURED )
  {
    DCD_EP_Tx (pdev, HID_IN_EP, report, len);
  }
  return USBD_OK;
}

/**
  * @brief  USBD_HID_GetCfgDesc 
  *         return configuration descriptor
  * @param  speed : current device speed
  * @param  length : pointer data length
  * @retval pointer to descriptor buffer
  */
static uint8_t  *USBD_HID_GetCfgDesc (uint8_t speed, uint16_t *length)
{
  *length = sizeof (USBD_HID_CfgDesc);
  return USBD_HID_CfgDesc;
}

/**
  * @brief  USBD_HID_DataIn
  *         handle data IN Stage
  * @param  pdev: device instance
  * @param  epnum: endpoint index
  * @retval status
  */
static uint8_t  USBD_HID_DataIn (void  *pdev, 
                              uint8_t epnum)
{
  
  /* Ensure that the FIFO is empty before a new transfer, this condition could 
  be caused by  a new transfer before the end of the previous transfer */
  DCD_EP_Flush(pdev, HID_IN_EP);
  return USBD_OK;
}

/**
  * @brief  USBD_HID_DataOut
  *         handle data OUT Stage
  * @param  pdev: device instance
  * @param  epnum: endpoint index
  * @retval status
  */

static uint8_t  USBD_HID_DataOut (void *pdev, uint8_t epnum)
{
	//uint16_t USB_RecData_Cnt;
//	BitAction Led_State;
//	uint16_t PortValue;
	if (epnum == HID_OUT_EP)
	{
		/* Get the received data buffer and update the counter */
		//USB_RecData_Cnt = ((USB_OTG_CORE_HANDLE*)pdev)->dev.out_ep[epnum].xfer_count;
		/* USB data will be immediately processed, this allow next USB traffic being
		   NAKed till the end of the application Xfer */
		if (((USB_OTG_CORE_HANDLE*)pdev)->dev.device_status == USB_OTG_CONFIGURED )
		{
			/* process the report setting */
			if (Buffer[0] == 1)
			{		// SW1
					if(SW1_Value != Buffer[1])
					{  	// if need to change state
							SW1_Value = Buffer[1];
							// Read current port value
							GPIOA->ODR &= ~(GPIO_Pin_1 | GPIO_Pin_3);
							GPIOA->BSRRL = (SW1_Value&0x01) << 1 | (SW1_Value&0x02) << (3-1);
							GPIOC->ODR &= ~GPIO_Pin_2;
							GPIOC->BSRRL = (SW1_Value&0x04);
					}
			}
			else if (Buffer[0] == 2)
			{		// SW2
					if(SW2_Value != Buffer[1])
					{  	// if need to change state
							SW2_Value = Buffer[1];
							// Read current port value
							GPIOE->ODR &= ~(GPIO_Pin_11 | GPIO_Pin_10 | GPIO_Pin_7);
							GPIOE->BSRRL = (SW2_Value&0x01) << 11 | (SW2_Value&0x02) << (10-1) | (SW2_Value&0x04) << (7-2);
					}
			}
			else if (Buffer[0] == 3)
			{		// SW3 control
					if(SW3_Value != Buffer[1])
					{  	// if need to change state
							SW3_Value = Buffer[1];
							// Read current port value
							GPIOB->ODR &= ~(GPIO_Pin_12 | GPIO_Pin_14 | GPIO_Pin_15);
							GPIOB->BSRRL = (SW3_Value&0x01) << 12 | (SW3_Value&0x02) << (14-1) | (SW3_Value&0x08) << (15-3);
							GPIOD->ODR &= ~GPIO_Pin_8;
							GPIOD->BSRRL = (SW3_Value&0x04) << (8-2);
					}
			}
			if (Buffer[0] == 4)	
			{		// SW4
					if(SW4_Value != Buffer[1])
					{  	// if need to change state
							SW4_Value = Buffer[1];
							// Read current port value
							GPIOD->ODR &= ~(GPIO_Pin_3 | GPIO_Pin_1 | GPIO_Pin_2);
							GPIOD->BSRRL = (SW4_Value&0x01) << 3 | (SW4_Value&0x02) | (SW4_Value&0x04);
					}
			}
			else if (Buffer[0] == 5)
			{		// SW5
					if(SW5_Value != Buffer[1])
					{  	// if need to change state
							SW5_Value = Buffer[1];
							// Read current port value
							GPIOC->ODR &= ~(GPIO_Pin_8 | GPIO_Pin_9 | GPIO_Pin_6);
							GPIOC->BSRRL = (SW5_Value&0x01) << 8 | (SW5_Value&0x02) << (9-1) | (SW5_Value&0x04) << (6-2);
					}
			}
			else if (Buffer[0] == 6)
			{		// SW6 control
					if(SW6_Value != Buffer[1])
					{  	// if need to change state
							SW6_Value = Buffer[1];
							// Read current port value
							GPIOE->ODR &= ~(GPIO_Pin_12 | GPIO_Pin_13 | GPIO_Pin_14 | GPIO_Pin_15);
							GPIOE->BSRRL = (SW6_Value&0x01) << 12 | (SW6_Value&0x02) << (13-1) | (SW6_Value&0x04) << (15-2) | (SW6_Value&0x08) << (14-3);
					}
			}
			else if (Buffer[0] == 11)
			{		// MUX16A
					if(MUX16A_Value != Buffer[1])
					{  	// if need to change state
							MUX16A_Value = Buffer[1];
							// Read current port value
							GPIOA->ODR &= ~(GPIO_Pin_8 | GPIO_Pin_15);
							GPIOA->BSRRL = (MUX16A_Value&0x04) << (15-2) | (MUX16A_Value&0x08) << (8-3);
							GPIOC->ODR &= ~(GPIO_Pin_11);
							GPIOC->BSRRL = (MUX16A_Value&0x02) << (11-1);
							GPIOD->ODR &= ~GPIO_Pin_0;
							GPIOD->BSRRL = (MUX16A_Value&0x01);
					}
			}
			else if (Buffer[0] == 13)		// SW_EN
			{		// 
				if(SW_EN != Buffer[1])
				{  	// if need to change state
					SW_EN = Buffer[1];	// Update current state
					if(Buffer[1] == 1)	// SW_EN ON
					{
						GPIOB->ODR |= 0x0004;
					}
					else // Switch power OFF
					{
						GPIOB->ODR &= ~0x0004;
					}
				}
			}
			else if (Buffer[0] == 14)		// PG_CTL
			{		// 
				if(PG_CTL != Buffer[1])
				{  	// if need to change state
					PG_CTL = Buffer[1];	// Update current state
					if(Buffer[1] == 1)	// PG_CTL -> 1
					{
						GPIOD->ODR |= 0x0400;
					}
					else // PG_CTL -> 0
					{
						GPIOD->ODR &= ~0x0400;
					}
				}
			}
			else if (Buffer[0] == 15)		// POWER_ON
			{		// 
				if(Power_ON != Buffer[1])
				{  	// if need to change state
					Power_ON = Buffer[1];	// Update current state
					if(Buffer[1] == 1)	// Switch power ON
					{
						GPIOC->ODR &= ~0x0010;
					}
					else // Switch power OFF
					{
						GPIOC->ODR |= 0x0010;
					}
				}
			}
			else if (Buffer[0] == 17)		// MUX_EN
			{		// 
				if(MUX_EN != Buffer[1])
				{  	// if need to change state
					MUX_EN = Buffer[1];	// Update current state
					if(Buffer[1] == 1)	// MUX_EN ON
					{
						GPIOB->ODR |= 0x0001;
					}
					else // Switch power OFF
					{
						GPIOB->ODR &= ~0x0001;
					}
				}
			}
			else if (Buffer[0] == 18)		// FAN_POWER
			{		// 
				if(Fan_Power != Buffer[1])
				{  	// if need to change state
					Fan_Power = Buffer[1];	// Update current state
					if(Buffer[1] == 1)			// FAN_POWER ON
					{
						GPIOB->ODR |= 0x2000;
					}
					else // Switch power OFF
					{
						GPIOB->ODR &= ~0x2000;
					}
				}
			}

			/* Prepare Out endpoint to receive next packet */
			DCD_EP_PrepareRx(pdev,
			                   HID_OUT_EP,
			                   (uint8_t*)(Buffer),
			                   HID_OUT_PACKET);
		}
	}
	return USBD_OK;
}


/**
  * @}
  */ 


/**
  * @}
  */ 


/**
  * @}
  */ 

/******************* (C) COPYRIGHT 2011 STMicroelectronics *****END OF FILE****/
