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
**/

#include "usbd_hid_core.h"
#include "usbd_desc.h"
#include "usbd_req.h"
#include "outputs.h"

int tx;
int rx;

static uint8_t  USBD_HID_Init (void  *pdev, 
                               uint8_t cfgidx);

static uint8_t  USBD_HID_DeInit (void  *pdev, 
                                 uint8_t cfgidx);

static uint8_t  USBD_HID_Setup (void  *pdev, 
                                USB_SETUP_REQ *req);

static uint8_t  *USBD_HID_GetCfgDesc (uint8_t speed, uint16_t *length);

static uint8_t  USBD_HID_DataIn (void  *pdev, uint8_t epnum);

static uint8_t  USBD_HID_DataOut (void  *pdev, uint8_t epnum);

void ClearBigRXorBigTX_MCU2(void);
void ClearRXorTX_MCU2(void);
void ProcessReport_MCU2(void);


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
			#ifdef MCU_1
			ClearRXorTX_MCU1();
			ProcessReport_MCU1();
			#else		
			ClearBigRXorBigTX_MCU2();
			ClearRXorTX_MCU2();
			ProcessReport_MCU2();
			#endif
			/* process the report setting 	*/
		}

			/* Prepare Out endpoint to receive next packet */
			DCD_EP_PrepareRx(pdev,
			                   HID_OUT_EP,
			                   (uint8_t*)(Buffer),
			                   HID_OUT_PACKET);
	
	}
	return USBD_OK;
}


void ClearBigRXorBigTX_MCU2()
{
	    // Resets BigRX or BigTX pins if new BigRX or BigTX report comes in
	    if (Buffer[0] == 5)
			{
				  GPIOB->ODR &= ~(GPIO_Pin_14 | GPIO_Pin_12);
				  GPIOD->ODR &= ~(GPIO_Pin_10 | GPIO_Pin_11);
					GPIOC->ODR &= ~(GPIO_Pin_11);
			}
			else if (Buffer[0] == 10)
			{
				  GPIOB->ODR &= ~(GPIO_Pin_15);
				  GPIOD->ODR &= ~(GPIO_Pin_8);
				  GPIOE->ODR &= ~(GPIO_Pin_12 | GPIO_Pin_14);
			}
}

void ClearRXorTX_MCU2()
{
	    // Resets RX or TX pins if new RX or TX report comes in, respectively
	    if (Buffer[0] == 1 || Buffer[0] == 2 || Buffer[0] == 3 || Buffer[0] == 4)
			{
				  GPIOB->ODR &= ~(GPIO_Pin_1 | GPIO_Pin_0);
				  GPIOC->ODR &= ~(GPIO_Pin_4 | GPIO_Pin_5 | GPIO_Pin_14);
				  GPIOE->ODR &= ~(GPIO_Pin_4 | GPIO_Pin_5 | GPIO_Pin_6);
			}
			else if (Buffer[0] == 6 || Buffer[0] == 7 || Buffer[0] == 8 || Buffer[0] == 9)
			{
				  GPIOB->ODR &= ~(GPIO_Pin_8 | GPIO_Pin_7 | GPIO_Pin_13);
				  GPIOE->ODR &= ~(GPIO_Pin_9 | GPIO_Pin_10 | GPIO_Pin_15);
			}
}

void ProcessReport_MCU2()
{
	if (Buffer[0] == 1)
	{		
		GPIO_SetBits(GPIOD, GPIO_Pin_14);
		// The tx value received through USB
		tx = (int)Buffer[1];
		tx = tx - 1;
		set_bits(tx, rx);
	}
	else if (Buffer[0] == 2)
	{		
		GPIO_SetBits(GPIOD, GPIO_Pin_12);
		// The rx value received through USB
		rx = (int)Buffer[1];
		rx = rx - 1;
		set_bits(tx, rx);
	}
}

